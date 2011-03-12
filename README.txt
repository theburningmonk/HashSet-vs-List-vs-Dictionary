A set of simple tests to compare the relative performance of HashSet<T>, List<T> and Dictionary<TKey, TValue>.

The tests cover the simple Add, Remove, and contains test for both value and reference types.

TESTS:
Test 1 - Add 1000000 value type objects with no duplicacy check

Test 2 - Add 1000000 reference type objects with no duplicacy check

Test 3 - Run Contains() method against half the objects in a list of 10000 value type objects

Test 4 - Run Contains() method against half the objects in a list of 10000 reference type objects

Test 5 - Remove half the items in a list of 10000 value types

Test 6 - Remove half the items in a list of 10000 reference types