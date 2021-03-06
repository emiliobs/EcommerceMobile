﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceMobile.Models;
using ECommerceMobile.Pages;
using ECommerceMobile.ViewModel;

namespace ECommerceMobile.Service
{
    public class NavigationService
    {

        #region MyRegion

        private DataService dataService;
        #endregion


        #region Constructor

        public NavigationService()
        {
            dataService = new DataService();
        }
        #endregion

        #region Methods

        public async Task Navigate(string pageName)
        {
            //Para el cirre de la ventana lateral despues del click:
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case "CustomersPage":
                    await App.Navigator.PushAsync(new CustomersPage());
                    break;
                case "DeliveriesPage":
                    await App.Navigator.PushAsync(new DeliveriesPage());
                    break;
                case "OrdersPage":
                    await App.Navigator.PushAsync(new OrdersPage());
                    break;
                case "ProductsPage":
                    await App.Navigator.PushAsync(new ProductsPage());
                    break;
                case "SetupPage":
                    await App.Navigator.PushAsync(new SetupPage());
                    break;
                case "SyncPage":
                    await App.Navigator.PushAsync(new SyncPage());
                    break;
                case "UsersPage":
                    await App.Navigator.PushAsync(new UsersPage());
                    break;
                case "CustomerDetailPage":
                    await App.Navigator.PushAsync(new CustomerDetailPage());
                    break;
                case "NewCustomerPage":
                    await App.Navigator.PushAsync(new  NewCustomerPage());
                    break;
                case "LogOutPape":
                    LogOut();
                    break;
                default:
                    break;

            }
        }

        private void LogOut()
        {
            App.CurrentUser.IsRemembered = false;

            //aqui actualizo el suario:
            dataService.UpdateUser(App.CurrentUser);

            //aqui cuando ingrese a la aplicación lo envía al loginPaige:
            App.Current.MainPage = new LoginPage();

        }

        public void SetMainPage(User user)
        {

            //aqui consumo el sigleton de MainViewMOdel(lapropiedady el metodo sin instancia la clase)
            //tomo la propiedad
            var mainViewModel = MainViewModel.GetInstance();

            //aqui tomo el método:
            mainViewModel.LoadUser(user);

            //Aqui le envio todos los usuarios a la clase principal App.cs:
            App.CurrentUser = user;

            App.Current.MainPage = new MasterPage();
        }

        public User GetCurrentUser()
        {
            return App.CurrentUser;
        }

        public async Task Back()
        {
            await App.Navigator.PopAsync();
        }

        #endregion


    }
}
