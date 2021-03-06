﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ECommerceMobile.Clasess;
using ECommerceMobile.Models;
using ECommerceMobile.Service;
using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ECommerceMobile.ViewModel
{
    public class CustomerItemView : Customer, INotifyPropertyChanged
    {

        #region Attributes

        private NavigationService navigationService;
        private NetService netService;
        private ApiService apiService;
        private DataService dataService;
        private DialogService dialogService;
        private ImageSource imageSource;
        private GeolocatorService geolocatorService;
        private bool isRunning;
        private double latitude;
        private double longitude;
        private MediaFile file;


        #endregion


        #region Properties

        public ObservableCollection<DepartmentItemViewModel> Departments { get; set; }
        public ObservableCollection<City> Cities { get; set; }

        public ImageSource ImageSource
        {

            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }

            get { return imageSource; }
        }

        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }

            get { return isRunning; }
        }

        public new double Latitude
        {
            set
            {
                if (latitude != value)
                {

                    latitude = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                }
            }

            get { return latitude; }

        }

        public new double Longitude
        {
            set
            {
                if (longitude != value)
                {

                    longitude = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                }
            }

            get { return longitude; }

        }



        #endregion

        #region Constructor

        public CustomerItemView()
        {
            //servicios
            navigationService = new NavigationService();
            netService = new NetService();
            dataService = new DataService();
            apiService = new ApiService();
            dialogService = new DialogService();
            geolocatorService = new GeolocatorService();

            //Observable collection:
            Departments = new ObservableCollection<DepartmentItemViewModel>();
            Cities = new ObservableCollection<City>();




            //LoadData:
            LoadDepartments();
            LoadCities();
        }



        #endregion

        #region Commands


        public ICommand UpdateCustomerLOcationCommand
        {
            get { return new RelayCommand(UpdateCustomerLocation); }
        }
        public async void UpdateCustomerLocation()


        {
            isRunning = true;

            await geolocatorService.getLocation();

            //var customer = dataService.Find<Customer>(CustomerId, false);
            var customer = dataService.Get<Customer>(false).FirstOrDefault(c => c.UserName == UserName);

            if (customer != null && geolocatorService.Latitude != 0 && geolocatorService.Longitud != 0)
            {

                Latitude = geolocatorService.Latitude;
                Longitude = geolocatorService.Longitud;

                customer.Latitude = geolocatorService.Latitude;
                customer.Longitude = geolocatorService.Longitud;

                //actualizo dsos locales
                dataService.Update(customer);


                var response = await apiService.Update(customer, "Customers");


            }

            isRunning = false;

        }

        public ICommand NewCustomerCommand
        {
            get { return new RelayCommand(NewCustomer); }
        }

        private async void NewCustomer()
        {
            //validar los campos:


            if (!Utilities.IsValidEmail(UserName))
            {
                await dialogService.ShowMessage("Error", "Debe ingresar un correo valido.");

                return;
            }

            if (string.IsNullOrEmpty(FirstName))
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar nombres.");
                return;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar un apellidos.");
                return;
            }

            if (string.IsNullOrEmpty(Phone))
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar un teléfono.");
                return;
            }

            if (string.IsNullOrEmpty(Address))
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar un dirección.");
                return;
            }

            if (DepartmentId == 0)
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar un departamento.");
                return;
            }

            if (CityId == 0)
            {
                await dialogService.ShowMessage("Error", "Debe ingrsar una ciudad.");
                return;
            }

            IsRunning = true;

            await geolocatorService.getLocation();

            var customer = new Customer()
            {
                CityId = CityId,
                DepartmentId = DepartmentId,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Address = Address,
                IsUpdated = IsUpdated,
                Latitude = geolocatorService.Latitude,
                Longitude = geolocatorService.Longitud,
                Phone = Phone,


            };

            var response = await apiService.NewCustomer(customer);

            if (response.IsSuccess && file != null)
            {
                var newCustomer = (Customer)response.Result;
                var response2 = await apiService.SetPhoto(newCustomer.CustomerId, file.GetStream());
                var fileName = $"{newCustomer.CustomerId}.jpg";
                var folder = "~/Content/Customers";
                var fullPath = $"({folder}/{fileName})";

                customer.Photo = fullPath;

                var response3 = await apiService.Update(customer, "Customers");
            }

            IsRunning = false;

            //aqui pregunso si genoro o no el nuevo cliente

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await dialogService.ShowMessage("Confirmación", response.Message);

            //aqui no envia ala pagina atras, osea la pagina index:
            await navigationService.Back();

        }


        public ICommand TakePictureCommand
        {
            get { return new RelayCommand(TakePicture); }
        }

        private async void TakePicture()
        {
            isRunning = true;

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await dialogService.ShowMessage("Error", "So se puede acceder a la camara.");
                return;
            }

            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Photos",
                Name = "newCustomer.jpg"
            });

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    // file.Dispose();
                    return stream;
                });
            }


            isRunning = false;


        }



        public ICommand CustomerDetailCommand
        {
            get
            {
                return new RelayCommand(customerDetail);
            }
        }



        private async void customerDetail()
        {
            var customerItemViewModel = new CustomerItemView()
            {
                Photo = Photo,
                FirstName = FirstName,
                LastName = LastName,
                Phone = Phone,
                Department = Department,
                City = City,
                UserName = UserName,
                Address = Address,
                CityId = CityId,
                CompanyCustomers = CompanyCustomers,
                CustomerId = CustomerId,
                DepartmentId = DepartmentId,
                IsUpdated = IsUpdated,
                Latitude = Latitude,
                Longitude = Longitude,
                Orders = Orders,
                Sales = Sales
            };

            //Singleton instancia del mismo objeto>
            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.SetCurrentCustomer(customerItemViewModel);

            await navigationService.Navigate("CustomerDetailPage");

        }

        #endregion

        #region Methods

        private async void LoadCities()
        {
            var cities = new List<City>();

            if (netService.IsConnected())
            {
                cities = await apiService.Get<City>("Cities");

                dataService.Save(cities);
            }
            else
            {
                cities = dataService.Get<City>(true);
            }

            ReloadCities(cities);
        }

        private void ReloadCities(List<City> cities)
        {
            Cities.Clear();

            foreach (var city in cities.OrderBy(c => c.Name))
            {
                Cities.Add(new CityItemViewModel()
                {
                    CityId = city.CityId,
                    Customers = city.Customers,
                    Name = city.Name,
                    DepartmentId = city.DepartmentId,
                    Department = city.Department


                });
            }
        }

        private async void LoadDepartments()
        {
            var departments = new List<Department>();

            if (netService.IsConnected())
            {
                departments = await apiService.Get<Department>("Departments");

                dataService.Save(departments);
            }
            else
            {
                departments = dataService.Get<Department>(true);
            }

            ReloadDeparments(departments);
        }

        private void ReloadDeparments(List<Department> departments)
        {
            Departments.Clear();

            foreach (var department in departments.OrderBy(d => d.Name))
            {
                Departments.Add(new DepartmentItemViewModel()
                {
                    Cities = department.Cities,
                    Customers = department.Customers,
                    DepartmentId = department.DepartmentId,
                    Name = department.Name
                });
            }
        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;



        #endregion

    }
}
