﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceMobile.Data;
using ECommerceMobile.Models;

namespace ECommerceMobile.Service
{

    public class DataService
    {

        #region Methods


        //aqui actualizo el usuario en la bd:
        public Response UpdateUser(User user)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    da.Update(user);
                }

                return new Response()
                {
                    IsSuccess = true,
                    Message = "Usuario Insertado, OK",
                    Result = user
                };
            }
            catch (Exception ex)
            {


                return new Response()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        //Metodo que nos deviulve el usario:
        public User GetUser()
        {
            using (var da = new DataAccess())
            {
                return da.First<User>(true);
            }


        }

        public Response InsertUser(User user)
        {
            try
            {




                using (var da = new DataAccess())
                {

                    //verifico que no exista un usuario ya logiado:, y si existe lo borro de la memoria.
                    var oldUser = da.First<User>(false);

                    if (oldUser != null)
                    {
                        da.Delete(oldUser);
                    }

                    da.Insert(user);
                }


                return new Response()
                {
                    IsSuccess = true,
                    Message = "Usuario Insertado, OK.!",
                    Result = user
                };
            }
            catch (Exception ex)
            {

                return new Response()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }


        //Lo voy a utilizar como un generico:
        //public void SaveProducts(List<Product> productasList)
        //{
        //    using (var da = new DataAccess())
        //    {
        //        //primero borro los datos viejos:
        //        var oldProducts = da.GetList<Product>(false);

        //        foreach (var product in oldProducts)
        //        {
        //            da.Delete(product);
        //        }

        //        //aqui guardo los produxtos nuevos en la bd:
        //        foreach (var product in productasList)
        //        {
        //            da.Insert(product);
        //        }

        //    }
        //}

        //Método normal:

        public List<Product> GetProducts(string filter)
        {
            using (var da = new DataAccess())
            {
                return
                    da.GetList<Product>(true)
                        .Where(p => p.Description.ToUpper().Contains(filter.ToUpper()) || p.BarCode.Contains(filter))
                        .OrderBy(p => p.Description)
                        .ToList();
            }
        }




        //Lo elimino por que voy a usar el método como generico:
        //public List<Product> GetProducts()
        //{
        //    using (var da = new DataAccess())
        //    {
        //        return da.GetList<Product>(true).OrderBy(p => p.Description).ToList();
        //    }
        //}

        public Response Login(string email, string password)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    //aqui busco si hay usuario en memoria o bd:
                    var user = da.First<User>(true);

                    if (user == null)
                    {
                        return new Response()
                        {
                            IsSuccess = false,
                            Message = "No hay conexión o internet, y no hay usuario previamente registrado.!"
                        };
                    }

                    //aqui hay conexio, y si el usuario  y contraseña es correcto pasa del login a userpage
                    if (user.UserName.ToUpper() == email.ToUpper() && user.Password == password)
                    {
                        return  new Response()
                        {
                            IsSuccess = true,
                            Message = "Login, OK.!",
                            Result = user
                        };
                    }

                    //aqui el hay problemas con usuario y paswword:
                    return new Response()
                    {
                        IsSuccess = false,
                        Message = "Usuario o Contraseña Incorrectos.!",

                    };


                }
            }
            catch (Exception ex)
            {

                return new Response()
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public List<Customer> GetCustomers(string customersFilter)
        {
            using (var da = new DataAccess())
            {
                return da.GetList<Customer>(true)
                    .Where(c => c.FirstName.ToUpper().Contains(customersFilter.ToUpper()) ||
                           c.LastName.ToUpper().Contains(customersFilter.ToUpper()))
                    .OrderBy(c=>c.FirstName)
                    .ThenBy(c => c.LastName)
                    .ToList();
            }
        }

        //public void SaveCustomers(List<Customer> customersList)
        //{
        //    using (var da = new DataAccess())
        //    {
        //        //primero borro los datos viejos:
        //        var oldCustomes = da.GetList<Customer>(false);

        //        foreach (var customer in oldCustomes)
        //        {
        //            da.Delete(customer);
        //        }

        //        //aqui guardo los produxtos nuevos en la bd:
        //        foreach (var customer in customersList)
        //        {
        //            da.Insert(customer);
        //        }

        //    }
        //}

        //Método Generico:
        public void Save<T>(List<T> list) where  T : class
        {
            using (var da = new DataAccess())
            {
                //primero borro los datos viejos:
                var oldRecords= da.GetList<T>(false);

                foreach (var record in oldRecords)
                {
                    da.Delete(record);
                }

                //aqui guardo los produxtos nuevos en la bd:
                foreach (var record in list)
                {
                    da.Insert(record);
                }

            }
        }

        //Método normal
        //public List<Customer> GetCustomers()
        //{
        //    using (var da = new DataAccess())
        //    {
        //        return da.GetList<Customer>(true).OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ToList();
        //    }
        //}

        //Método Generico
        public List<T> Get<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                //ya no lo puedo ordenar, que lo ordenes quien lo use:
                return da.GetList<T>(withChildren).ToList();
            }
        }

        public T Find<T>(int pk, bool wintChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.Find<T>(pk, wintChildren);
            }
        }

        public void Update<T>(T model)
        {
            using (var db = new DataAccess())
            {
               db.Update(model);
            }
        }
    }


    #endregion
}
