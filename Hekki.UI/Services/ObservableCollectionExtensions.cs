using System.Collections.ObjectModel;

namespace Hekki.UI.Services
{
    public static class ObservableCollectionExtensions
    {
        public static void ReorderBy<T, TKey>(this ObservableCollection<T> collection, IReadOnlyList<TKey> orderedKeys, Func<T, TKey> keySelector)
        {
            var comparer = EqualityComparer<TKey>.Default;

            for (var target = 0; target < orderedKeys.Count && target < collection.Count; target++)
            {
                for (var current = target; current < collection.Count; current++)
                {
                    if (!comparer.Equals(keySelector(collection[current]), orderedKeys[target]))
                        continue;

                    if (current != target)
                        collection.Move(current, target);
                    break;
                }
            }
        }
    }
}
