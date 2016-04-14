using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToolLibrary.DAL;

namespace ToolLibrary.Logic
{
    public class BasketActions : IDisposable
    {
        private ToolDbContext _db = new ToolDbContext();
        string UserId { get; set; }
        int count { get; set; }

        public int GetCount(string userID)
        {
            var rentals = _db.Rentals.Where(c => c.UserID == userID).ToList();
            if (!rentals.Count.Equals(0))
                return rentals.Count;
            else
                return 0;
        }

        public void add()
        {
            count++;
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }
    }
}