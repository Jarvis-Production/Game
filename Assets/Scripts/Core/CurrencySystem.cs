using System;
using UnityEngine;

namespace TrainSurvival.Core
{
    public class CurrencySystem : MonoBehaviour
    {
        [SerializeField] private int startingCurrency = 120;

        public int Current { get; private set; }
        public event Action<int> OnCurrencyChanged;

        private void Awake()
        {
            Current = startingCurrency;
            OnCurrencyChanged?.Invoke(Current);
        }

        public void Add(int amount)
        {
            Current += Mathf.Max(0, amount);
            OnCurrencyChanged?.Invoke(Current);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
                return true;

            if (Current < amount)
                return false;

            Current -= amount;
            OnCurrencyChanged?.Invoke(Current);
            return true;
        }
    }
}
