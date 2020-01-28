using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PizzaBox.Storing.Entities;

namespace PizzaBox.Storing.Interface
{
    public interface IPizzaboxRepository : IDisposable
    {
        IEnumerable<Inventory> GetInventoryByType(string type = "preset", int search = 0); 

        Inventory GetInventoryByStore(int storeid); 

        int AddInventory(Inventory inventory); 

        void UpdateInventory(int id, int preset, int custom, string type);  

        IEnumerable<Orders> GetOrdersBy(string search, string type = "user");

        IEnumerable<Ordertype> GetAllOrdertypes();

        Ordertype GetOrdertypeById(int id);

        void AddOrder(Orders orders, Ordertype ordertype, string preset, string custom); 

        IEnumerable<Store> GetAllStores();  

        Store GetStoreById(int id); 

        void AddStore(Store store); 

        IQueryable GetUsersByStoreId(int id);

        User GetUserById(string uname);

        User UserAuthentication(string uname, string pass);

        void AddUser(User user);   

        void UpdateUser(User user);

        void save();
    }
}
