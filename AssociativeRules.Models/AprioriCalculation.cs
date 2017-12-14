using System.Collections.Generic;
using System.Linq;

namespace AssociativeRules.Models
{
    public class AprioriCalculation
    {
        public AprioriCalculation()
        {
            _transactions = new Dictionary<string, List<string>>();
            MinSupport = 0;
            MinConfidence = 0;
        }

        public List<AssociativeRule> GenerateRules()
        {
            var sets = GenerateSets();
            List<AssociativeRule> rules = new List<AssociativeRule>();
            int transactionsCount = _transactions.Count;

            foreach (var set in sets)
            {
                int setCount = set.Key.Count;

                if (setCount < 2)
                {
                    continue;
                }

                int iteration = 1;

                while (iteration < setCount)
                {
                    var ifItems = MathUtil.Combinations(set.Key, setCount-iteration);

                    foreach (var ifItem in ifItems)
                    {
                        List<string> thenItem = set.Key.FindAll(i => !ifItem.Any(ifI => ifI == i));

                        double confidence = (set.Value * 100.0) / (ItemAppearanceCount(ifItem) * 100.0 / (double)transactionsCount);

                        if (confidence >= MinConfidence)
                        {
                            rules.Add(new AssociativeRule()
                            {
                                Confidence = confidence,
                                IfSet = ifItem.ToList(),
                                ThenSet = thenItem
                            });
                        }
                    }

                    iteration++;
                }
            }

            return rules;
        }
        public Dictionary<List<string>, double> GenerateSets()
        {
            List<string> items = GetItems();
            int transactionsCount = _transactions.Count;
            Dictionary<List<string>, double> supportedItems = new Dictionary<List<string>, double>();

            IEnumerable<IEnumerable<string>> currentSet;
            int iteration = 1;

            do
            {
                currentSet = MathUtil.Combinations(items, iteration);

                foreach (var item in currentSet)
                {
                    double itemsSupport = ItemAppearanceCount(item) * 100.0 / (double)transactionsCount;

                    if (itemsSupport >= MinSupport)
                    {
                        if (iteration > 1)
                        {
                            bool isValidSet = true;
                            var subSets = MathUtil.Combinations(item, iteration - 1);

                            foreach (var subItem in subSets)
                            {
                                double subItemsSupport = ItemAppearanceCount(item) * 100.0 / (double)transactionsCount;
                                if (subItemsSupport < MinSupport)
                                {
                                    isValidSet = false;
                                    break;
                                }
                            }

                            if (isValidSet)
                            {
                                supportedItems.Add(item.ToList(), itemsSupport);
                            }
                        }
                        else
                        {
                            supportedItems.Add(item.ToList(), itemsSupport);
                        }
                    }
                }

                iteration++;
            } while (currentSet.Count() > 1);

            return supportedItems;
        }
        public List<string> GetItems()
        {
            List<string> items = new List<string>();

            foreach (var t in _transactions)
            {
                foreach (var item in t.Value)
                {
                    if (!items.Any(i => i == item))
                    {
                        items.Add(item);
                    }
                }
            }

            items.Sort();
            return items;
        }
        public int ItemAppearanceCount(IEnumerable<string> items)
        {
            int count = 0;

            foreach (var t in _transactions)
            {
                bool appeared = true;

                foreach (var item in items)
                {
                    if (!t.Value.Any(i => i == item))
                    {
                        appeared = false;
                        break;
                    }
                }

                if (appeared)
                {
                    count++;
                }
            }

            return count;
        }
        public void AddTransaction(string name, List<string> elements)
        {
            _transactions.Add(name, elements);
        }

        public double MinSupport { get; set; }
        public double MinConfidence { get; set; }

        private Dictionary<string, List<string>> _transactions;
    }

    public static class MathUtil
    {
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
                elements.SelectMany((e, i) =>
                    elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }
    }
}