﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace ECommerceMobile.Models
{
     public class User
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int CompanyId { get; set; }

        [ManyToOne]
        public Company Company { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }

        public  bool IsRemembered { get; set; }

        public string Password { get; set; }


        public string FullName
        {
            get { return $"{ FirstName } { LastName }"; }
        }

        public string PhotoFullPath
        {
            get { return $@"http://zulu-software.com/ECommerce{Photo.Substring(1)}"; }
        }

        public override int GetHashCode()
        {
            return UserId;
        }
    }
}
