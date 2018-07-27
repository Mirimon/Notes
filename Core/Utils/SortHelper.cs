using System;
using System.Collections.ObjectModel;

namespace SecurityNotes.Core.Utils {
    public static class SortHelper {
        public static void Sort<T>(ObservableCollection<T> collection, Func<T, T, bool> comparer) {
            if (collection == null)
                return;

            // This sort algorithm is used because it does minimal elements movings
            // Each element moving updates UI
            int maxElementIndex = 0;
            for (int i = 0; i < collection.Count; i++) {
                maxElementIndex = i;
                for (int j = i; j < collection.Count; j++) {
                    if (comparer(collection[j], collection[maxElementIndex])) {
                        maxElementIndex = j;
                    }
                }
                collection.Move(maxElementIndex, i);
            }
        }
    }
}
