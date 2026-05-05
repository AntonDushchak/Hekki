using Hekki.Application.Abstrations;
using Hekki.Domain.Models;
using Hekki.UI.Services;
using Hekki.UI.ViewModels;
using Moq;

namespace Hekki.UI.Tests
{
    public class SelectionViewModelTests
    {
        private Mock<IRegulationService> _mockRegulationService;
        private Mock<IViewModelFactory> _mockViewModelFactory;
        private Mock<INavigationService> _mockNavigationService;
        private Mock<IPaginationService> _mockPaginationService;
        private SelectionViewModel _viewModel;
        private List<Regulation> _testRegulations;

        [SetUp]
        public void Setup()
        {
            _mockRegulationService = new Mock<IRegulationService>();
            _mockViewModelFactory = new Mock<IViewModelFactory>();
            _mockNavigationService = new Mock<INavigationService>();
            _mockPaginationService = new Mock<IPaginationService>();

            _testRegulations =
            [
                new Regulation { Id = 1, Name = "Test Regulation 1" },
                new Regulation { Id = 2, Name = "Test Regulation 2" },
                new Regulation { Id = 3, Name = "Test Regulation 3" }
            ];

            _mockRegulationService
               .Setup(x => x.GetLookupAsync())
               .ReturnsAsync((IReadOnlyList<Regulation>)_testRegulations);

            _mockPaginationService
                .SetupGet(x => x.StartItem)
                .Returns(1);

            _mockPaginationService
                .SetupGet(x => x.PageCapacity)
                .Returns(10);

            _viewModel = new SelectionViewModel(
                _mockNavigationService.Object,
                _mockRegulationService.Object,
                _mockViewModelFactory.Object,
                _mockPaginationService.Object);
        }

        [Test]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.That(_viewModel.Regulations, Is.Not.Null);
            Assert.That(_viewModel.Regulations.Count, Is.EqualTo(0));
            Assert.That(_viewModel.PaginationService, Is.EqualTo(_mockPaginationService.Object));
            Assert.That(_viewModel.IsLoading, Is.False);
        }

        [Test]
        public async Task InitializeAsync_ShouldLoadRegulations()
        {
            // Act
            await _viewModel.InitializeAsync();

            // Assert
            Assert.That(_viewModel.Regulations.Count, Is.EqualTo(3));
            Assert.That(_viewModel.Regulations[0].Name, Is.EqualTo("Test Regulation 1"));
            Assert.That(_viewModel.IsLoading, Is.False);

            _mockRegulationService.Verify(x => x.GetLookupAsync(), Times.Once);
            _mockPaginationService.Verify(x => x.SetTotalItems(3), Times.Once);
        }

        [Test]
        public async Task InitializeAsync_ShouldSetLoadingState()
        {
            // Arrange - задерживаем выполнение сервиса
            var tcs = new TaskCompletionSource<IReadOnlyList<Regulation>>();
            _mockRegulationService
                .Setup(x => x.GetLookupAsync())
                .Returns(tcs.Task);

            // Act
            var initTask = _viewModel.InitializeAsync();

            // Assert
            Assert.That(_viewModel.IsLoading, Is.True);

            tcs.SetResult(_testRegulations);
            await initTask;

            // Assert
            Assert.That(_viewModel.IsLoading, Is.False);
        }

        [Test]
        public async Task InitializeAsync_ShouldHandleException()
        {
            // Arrange
            _mockRegulationService
                .Setup(x => x.GetLookupAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert - исключение не должно убить ViewModel
            Assert.DoesNotThrowAsync(() => _viewModel.InitializeAsync());

            // IsLoading должен быть сброшен даже при ошибке
            await _viewModel.InitializeAsync();
            Assert.That(_viewModel.IsLoading, Is.False);
        }

        [Test]
        public void PagedRegulations_ShouldReturnCorrectSubset()
        {
            // Arrange
            _mockPaginationService.SetupGet(x => x.StartItem).Returns(2);
            _mockPaginationService.SetupGet(x => x.PageCapacity).Returns(2);

            // Добавляем тестовые данные
            foreach (var regulation in _testRegulations)
            {
                _viewModel.Regulations.Add(regulation);
            }

            // Act
            var pagedRegulations = _viewModel.PagedRegulations;

            // Assert
            Assert.That(pagedRegulations.Count, Is.EqualTo(2));
            Assert.That(pagedRegulations[0].Id, Is.EqualTo(2));
            Assert.That(pagedRegulations[1].Id, Is.EqualTo(3));
        }

        [Test]
        public void NavigateToCreationCommand_ShouldCallNavigationService()
        {
            // Act
            _viewModel.NavigateToCreationCommand.Execute(null);

            // Assert
            _mockViewModelFactory.Verify(x => x.Create<CreateRaceViewModel>(), Times.Once);
            _mockNavigationService.Verify(x => x.Go(It.IsAny<CreateRaceViewModel>()), Times.Once);
        }

        [Test]
        public void NavigateToRaceCommand_ShouldCallNavigationServiceWithId()
        {
            // Arrange
            const int testId = 123;
            var mockRaceViewModel = new Mock<RaceViewModel>();
            _mockViewModelFactory
                .Setup(x => x.CreateRaceViewModel(testId))
                .Returns(mockRaceViewModel.Object);

            // Act
            _viewModel.NavigateToRaceCommand.Execute(testId);

            // Assert
            _mockViewModelFactory.Verify(x => x.CreateRaceViewModel(testId), Times.Once);
            _mockNavigationService.Verify(x => x.Go(mockRaceViewModel.Object), Times.Once);
        }

        [Test]
        public void PrevCommand_ShouldCallPaginationService()
        {
            // Act
            _viewModel.PrevCommand.Execute(null);

            // Assert
            _mockPaginationService.Verify(x => x.Prev(), Times.Once);
        }

        [Test]
        public void NextCommand_ShouldCallPaginationService()
        {
            // Act
            _viewModel.NextCommand.Execute(null);

            // Assert
            _mockPaginationService.Verify(x => x.Next(), Times.Once);
        }

        [Test]
        public void PaginationService_PropertyChanged_ShouldUpdatePagedRegulations()
        {
            // Arrange
            var propertyChangedFired = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.PagedRegulations))
                    propertyChangedFired = true;
            };

            // Act
            _mockPaginationService.Raise(x => x.PropertyChanged += null,
                new System.ComponentModel.PropertyChangedEventArgs("SomeProperty"));

            // Assert
            Assert.That(propertyChangedFired, Is.True);
        }

        [Test]
        public void Regulations_CollectionChanged_ShouldUpdatePagedRegulations()
        {
            // Arrange
            var propertyChangedFired = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.PagedRegulations))
                    propertyChangedFired = true;
            };

            // Act
            _viewModel.Regulations.Add(new Regulation { Id = 999, Name = "New Regulation" });

            // Assert
            Assert.That(propertyChangedFired, Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            // Очистка после каждого теста
            _viewModel = null;
        }
    }
}
