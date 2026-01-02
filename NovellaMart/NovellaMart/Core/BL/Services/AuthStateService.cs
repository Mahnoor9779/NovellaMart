using System;

namespace NovellaMart.Core.BL.Services
{
    public class AuthStateService
    {
        // This event triggers whenever login or logout happens
        public event Action OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}