namespace Lab10.Warehouse
{
    public class InventoryService
    {
        private int _availableItems = 10;
        private int _reservedItems = 0;
        
        private readonly object _lockObject = new();

        public (int Available, int Reserved) GetInventoryStatus()
        {
            lock (_lockObject)
            {
                return (_availableItems, _reservedItems);
            }
        }

        public void AddInventory(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            lock (_lockObject)
            {
                _availableItems += quantity;
            }
        }

        public bool TryReserveItems(int quantity)
        {
            lock (_lockObject)
            {
                if (_availableItems >= quantity)
                {
                    _availableItems -= quantity;
                    _reservedItems += quantity;
                    return true;
                }
                return false;
            }
        }

        public void ConfirmReservation(int quantity)
        {
            lock (_lockObject)
            {
                if (_reservedItems < quantity)
                    throw new InvalidOperationException("Not enough reserved items to confirm");

                _reservedItems -= quantity;
            }
        }

        public void CancelReservation(int quantity)
        {
            lock (_lockObject)
            {
                if (_reservedItems < quantity)
                    throw new InvalidOperationException("Not enough reserved items to cancel");

                _reservedItems -= quantity;
                _availableItems += quantity;
            }
        }
    }
}