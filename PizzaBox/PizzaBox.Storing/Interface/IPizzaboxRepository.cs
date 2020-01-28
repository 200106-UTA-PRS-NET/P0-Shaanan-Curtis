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
        //INENTORY
        IEnumerable<Inventory> GetInventoryByType(string type = "preset", int search = 0);  //for inventory method

        Inventory GetInventoryByStore(int storeid); //general

        int AddInventory(Inventory inventory); //after addstore method

        void UpdateInventory(int id, int preset, int custom, string type);  //after order confirmed

        IEnumerable<Orders> GetOrdersBy(string search, string type = "user");

        Ordertype GetOrdertypeById(int id);
       
        //IEnumerable GetOrders(string search, string type = "user");

        void AddOrder(Orders orders, Ordertype ordertype, string preset, string custom);  //after order confirmed

        //void UpdateOrders(Orders orders);   //if username change available in the future

        //IEnumerable<Ordertype> GetAllOrdertype(string search = null); 

        //Ordertype GetOrdertypeById(int id);

        //void AddOrdertype(Ordertype ordertype);

        //void UpdateOrdertype(Ordertype ordertype);  //if username change available in the future

        IEnumerable<Store> GetAllStores();   //location method

        Store GetStoreById(int id); //general

        void AddStore(Store store); //addstore method

        //void UpdateStore(Store store);    ///currently unnecessary

        IQueryable GetUsersByStoreId(int id);

        User GetUserById(string uname);

        User UserAuthentication(string uname, string pass);

        void AddUser(User user);    //sign up

        void UpdateUser(User user);

        void save();
    }
}
