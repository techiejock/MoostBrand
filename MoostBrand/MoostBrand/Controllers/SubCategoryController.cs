﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoostBrand.DAL;
using PagedList;
using System.Configuration;
using System.Data.Entity;

namespace MoostBrand.Controllers
{
    public class SubCategoryController : Controller
    {
        MoostBrandEntities entity = new MoostBrandEntities();
        // GET: SubCategory
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "code" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "desc" : "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "catdesc" : "";
            ViewBag.Categories = entity.Categories.ToList();

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var sub = from c in entity.SubCategories
                             select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                sub = sub.Where(c => c.Code.Contains(searchString)
                                       || c.Description.Contains(searchString)
                                       || c.Category.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "code":
                    sub = sub.OrderByDescending(c => c.Code);
                    break;
                case "desc":
                    sub = sub.OrderByDescending(c => c.Description);
                    break;
                case "catdesc":
                    sub = sub.OrderByDescending(c => c.Category.Description);
                    break;
                default:
                    sub = sub.OrderBy(c => c.ID);
                    break;
            }

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return View(sub.ToPagedList(pageNumber, pageSize));
        }

        // GET: SubCategory/Details/5
        public ActionResult Details(int id)
        {
            var sub = entity.SubCategories.Find(id);
            return View(sub);
        }

        // GET: SubCategory/Create
        public ActionResult Create()
        {
            ViewBag.Categories = entity.Categories.ToList();
            return View();
        }

        // POST: SubCategory/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var sub = new SubCategory();

                if (collection.Count > 0)
                {
                    sub.Code = collection["Code"];
                    sub.Description = collection["Description"];
                    sub.CategoryID = Convert.ToInt32(collection["CategoryID"]);

                    try
                    {
                        entity.SubCategories.Add(sub);
                        entity.SaveChanges();
                    }
                    catch (Exception err) { }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SubCategory/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Categories = entity.Categories.ToList();

            var sub = entity.SubCategories.Find(id);

            return View(sub);
        }

        // POST: SubCategory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var sub = entity.SubCategories.Find(id);

                if (collection.Count > 0)
                {
                    sub.Code = collection["Code"];
                    sub.Description = collection["Description"];
                    sub.CategoryID = Convert.ToInt32(collection["CategoryID"]);

                    try
                    {
                        entity.Entry(sub).State = EntityState.Modified;
                        entity.SaveChanges();
                    }
                    catch (Exception err) { }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SubCategory/Delete/5
        public ActionResult Delete(int id)
        {
            var sub = entity.SubCategories.Find(id);
            return View(sub);
        }

        // POST: SubCategory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var sub = entity.SubCategories.Find(id);

                try
                {
                    entity.SubCategories.Remove(sub);
                    entity.SaveChanges();
                }
                catch { }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
