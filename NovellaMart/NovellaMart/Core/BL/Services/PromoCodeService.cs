using NovellaMart.Core.BL.Model_Classes;
using NovellaMart.Core.BL.Data_Structures;
using NovellaMart.Core.DL;
using System;

namespace NovellaMart.Core.BL.Services
{
    public class PromoCodeService
    {
        private MyLinkedList<PromoCodeBL> _promoCodes;
        private const string FilePath = "promocodes.json";

        public PromoCodeService()
        {
            LoadPromoCodes();
        }

        private void LoadPromoCodes()
        {
            _promoCodes = FileHandler.LoadData<MyLinkedList<PromoCodeBL>>(FilePath);
            if (_promoCodes == null)
            {
                _promoCodes = new MyLinkedList<PromoCodeBL>();
            }
        }

        public MyLinkedList<PromoCodeBL> GetAllPromoCodes()
        {
            return _promoCodes;
        }

        public void CreatePromoCode(string code, int discount)
        {
            int newId = (int)(DateTime.Now.Ticks % int.MaxValue);
            var newPromo = new PromoCodeBL(newId, code.ToUpper(), discount);
            _promoCodes.InsertAtEnd(newPromo);
            Save();
        }

        public void DeletePromoCode(int id)
        {
            var current = _promoCodes.head;
            while (current != null)
            {
                if (current.Data.Id == id)
                {
                    _promoCodes.Remove(current.Data);
                    Save();
                    return;
                }
                current = current.Next;
            }
        }

        public PromoCodeBL ValidatePromoCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            
            var current = _promoCodes.head;
            while (current != null)
            {
                if (current.Data.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase) && current.Data.IsActive)
                {
                    return current.Data;
                }
                current = current.Next;
            }
            return null;
        }

        private void Save()
        {
            FileHandler.SaveData(FilePath, _promoCodes);
        }
    }
}
