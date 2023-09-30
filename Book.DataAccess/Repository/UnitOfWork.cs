﻿using Book.DataAccess.Data;
using Book.DataAccess.Repository.IRepository;
using Book.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDBContext _db;
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(ApplicationDBContext db)
        { 
            _db = db; 
            Category = new CategoryRepository(_db);
        }
       

        public void Save()
        {
           _db.SaveChanges();
        }
    }
}
