﻿using System;
using System.Linq;
using System.Web.Mvc;
using MoostBrand.DAL;
using PagedList;
using System.Data.Entity;
using System.Configuration;
using MoostBrand.Models;
using System.Web.Routing;
using System.Collections.Generic;

namespace MoostBrand.Controllers
{
    [LoginChecker]
    public class ReservationController : Controller
    {
        RequisitionDetailsRepository reqDetailRepo = new RequisitionDetailsRepository();
        InventoryRepository invRepo = new InventoryRepository();

        int UserID = 0;
        int UserType = 0;
        int ModuleID = 3;
        MoostBrandEntities entity;
        // GET: Customer

        #region COMMENTS

        //[HttpPost]
        //public JsonResult getCodeCommit(int Code)
        //{
        //    var instock = db.Inventories.Find(Code);
        //    var com = db.RequisitionDetails.Where(x => x.Requisition.RequisitionTypeID == 4 && x.AprovalStatusID == 2 && x.Requisition.LocationID == instock.LocationCode);
        //    var total = com.Sum(x => x.Quantity);
        //    if (total == null)
        //    {
        //        total = 0;
        //    }
        //    return Json(total, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult getCodePO(int Code)
        //{
        //    var instock = db.Inventories.Find(Code);
        //    var pur = db.RequisitionDetails.Where(x => x.Requisition.RequisitionTypeID == 1 && x.AprovalStatusID == 2 && x.Requisition.LocationID == instock.LocationCode);
        //    var total = pur.Sum(x => x.Quantity);
        //    if (total == null)
        //    {
        //        total = 0;
        //    }
        //    return Json(total, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region  method
        public ReservationController()
        {
            entity = new MoostBrandEntities();
        }

        private string CheckExistingRefNum(string referencenum)
        {
            var cref = entity.Requisitions.Where(r => r.RefNumber == referencenum);
            if (cref == null)
            {

            }
            else
            {
                referencenum = Generator("CR");
            }

            return referencenum;
        }
        private string Generator(string prefix)
        {
            startR: var cref = entity.Requisitions.Where(r => r.RefNumber.Contains("CR")).Count();
           
            string refnum = string.Format(prefix + "-{0:000000}", cref);

            var pr = entity.Requisitions.ToList().FindAll(p => p.RefNumber == refnum);
            if (pr.Count() > 0)
            {
                goto startR;
            }
           
           
            return refnum;
            //////Initiate objects & vars
            ////startR: Random random = new Random();
            ////string randomString = "";
            ////int randNumber = 0;

            //////Loop ‘length’ times to generate a random number or character
            ////for (int i = 0; i < 6; i++)
            ////{
            ////    if (i == 0)
            ////    {
            ////        start: randNumber = random.Next(0, 9); //int {0-9}
            ////        if (randNumber == 0)
            ////            goto start;
            ////    }
            ////    else
            ////    {
            ////        randNumber = random.Next(0, 9);
            ////    }
            ////    //append random char or digit to random string
            ////    randomString = randomString + randNumber.ToString();
            ////}

            ////randomString = prefix + "-" + randomString;
            ////var pr = entity.Requisitions.ToList().FindAll(p => p.RefNumber == randomString);
            ////if (pr.Count() > 0)
            ////{
            ////    goto startR;
            ////}

            //////return the random string
            ////return randomString;
        }
        private Requisition setValue(Requisition pr)
        {
            pr.VendorID = null;
            pr.ReqTypeID = 2;
            pr.RequisitionTypeID = 4;
            pr.PlateNumber = null;
            pr.Others = null;
            pr.TimeDeparted = null;
            pr.TimeArrived = null;
            if (pr.ShipmentTypeID == 2)
            {
                pr.DropShipID = null;
            }
            return pr;
        }
        private void AccessChecker(int action)
        {
            int UserID = Convert.ToInt32(Session["sessionuid"]);

            var access = entity.UserAccesses.FirstOrDefault(u => u.EmployeeID == UserID && u.ModuleID == ModuleID);

            if (action == 1) //CanView
            {
                if (!access.CanView)
                {
                    RedirectToAction("Index", "Home");
                }
            }
            else if (action == 2)//CanEdit
            {
                if (!access.CanEdit)
                {
                    RedirectToAction("Index", "Home");
                }
            }
            else if (action == 3)//CanDelete
            {
                if (!access.CanDelete)
                {
                    RedirectToAction("Index", "Home");
                }
            }
            else if (action == 4)//CanRequest
            {
                if (!access.CanRequest)
                {
                    RedirectToAction("Index", "Home");
                }
            }
            else if (action == 5)//Can Approve/Deny
            {
                if (!access.CanDecide)
                {
                    RedirectToAction("Index", "Home");
                }
            }
        }
        
        public int getPurchaseOrder(int itemID)
        {
            var requi = entity.RequisitionDetails.FirstOrDefault(x => x.AprovalStatusID == 2);
            int po = 0;
            if (requi != null)
            {
                var lstReqDetail = new List<RequisitionDetail>();

                lstReqDetail = entity.RequisitionDetails.Where(x => x.Requisition.RequisitionTypeID == 1 && x.AprovalStatusID == 2 && x.ItemID == itemID && x.Requisition.LocationID == requi.Requisition.LocationID).ToList();

                po = lstReqDetail.Sum(x => x.Quantity) ?? 0;
            }
            return po;
        }

        #endregion

        #region JSON
        [HttpPost]
        public JsonResult GetExp()
        {
            string message = "";
            string noErrJs = "";
            bool success = false;

            var lstreserved = entity.Requisitions.Where(x => x.Status == false && x.RequisitionTypeID == 4);

            foreach (var reserved in lstreserved)
            {
                Requisition _req = entity.Requisitions.Find(reserved.ID);
                DateTime startedDate = reserved.RequestedDate;
                DateTime validityDate = Convert.ToDateTime(reserved.ValidityOfReservation);
                DateTime notif = Convert.ToDateTime(reserved.DaysOfNotification);

                //DateTime expiryDateW = startedDate;
                DateTime expiryDateFD = startedDate.AddDays(7);
                if(reserved.PaymentStatusID == 1 && DateTime.Now == notif)
                {
                    noErrJs = "false";
                    message = "Days of notif";
                }
                if (reserved.PaymentStatusID == 1 && DateTime.Now > validityDate)
                {
                    _req.ReservationStatus = "Cancelled";
                    entity.Entry(reserved).State = EntityState.Modified;
                   // entity.SaveChanges();
                }
                else if (reserved.PaymentStatusID == 2 || reserved.PaymentStatusID == 3 || reserved.PaymentStatusID == 4 && DateTime.Now > expiryDateFD)
                {

                    noErrJs = "true";
                    message = "Reservation is required on" + startedDate;
                    //reserved.ReservationStatus = "Cancelled";
                    //db.Entry(lstreserved).State = EntityState.Modified;
                    //db.SaveChanges();
                }
            }
            return Json(new { message = message, noErrJs = noErrJs, success = success }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetCategories(string name)
        {
            var categories = entity.Categories.Where(x => x.Description.Contains(name))
                            .Select(x => new
                            {
                                ID = x.ID,
                                Name = x.Description
                            });
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItems(int catID, string name)
        {
            var items = entity.Items.Where(x => x.CategoryID == catID || x.Description.Contains(name))
                            .Select(x => new
                            {
                                ID = x.ID,
                                Code = x.Code,
                                Name = x.DescriptionPurchase,
                                UOM = x.UnitOfMeasurement.Description,
                                Category = x.Category.Description
                            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItemCode(int catID, string name)
        {
            var items = entity.Items.Where(x => x.CategoryID == catID || x.Code.Contains(name) && x.ItemStatus == 1)
                            .Select(x => new
                            {
                                ID = x.ID,
                                Code = x.Code,
                                Name = x.DescriptionPurchase,
                                UOM = x.UnitOfMeasurement.Description,
                                Category = x.Category.Description
                            });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //NEW
        [HttpGet]
        public JsonResult getInstock(int id, string Code, int ItemID)
        {
            int total = (reqDetailRepo.getInstocked(id, Code) - reqDetailRepo.getStockTranfer(ItemID));

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getCommit(int id, int ItemID)
        {
            int total = reqDetailRepo.getCommited(id, ItemID);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getPO(int id, int ItemID)
        {
            var requi = entity.Requisitions.Find(id);

            int total = reqDetailRepo.getPurchaseOrder(requi.LocationID.Value, ItemID);

            return Json(total, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region actionresult

        public ActionResult OrderCheckingIndex(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "reqno" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //var prs = db.Requisitions.Where(x => x.Status == false && x.RequisitionTypeID == 4); //active

            var prs = entity.Requisitions.Where(x => x.Status == false && x.RequisitionTypeID == 4 && x.ApprovalStatus == 2);

            if (!String.IsNullOrEmpty(searchString))
            {
                prs = prs.Where(o => o.RequisitionTypeID == 4 && o.RefNumber.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "reqno":
                    prs = prs.OrderByDescending(o => o.RefNumber);
                    break;
                default:
                    prs = prs.OrderBy(o => o.ID);
                    break;
            }

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return View(prs.ToPagedList(pageNumber, pageSize));
        }

        [AccessCheckerForDisablingButtons(ModuleID = 9)]
        [AccessChecker(Action = 1, ModuleID = 9)]

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {


            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "reqno" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //int locID = Convert.ToInt32(Session["locationID"]);
            //int UserID = Convert.ToInt32(Session["userID"]);

            //var user = db.Users.FirstOrDefault(x => x.ID == UserID);

            var prs = entity.Requisitions.Where(x => x.Status == false && x.RequisitionTypeID == 4); //active

            if (!String.IsNullOrEmpty(searchString))
            {
                prs = prs.Where(o => o.RequisitionTypeID == 4 && o.RefNumber.Contains(searchString) ||
                                                                   o.ApprovalStatu.Status.Contains(searchString) ||
                                                                   o.AuthorizedPerson.Contains(searchString) ||
                                                                    o.Customer.Contains(searchString) ||
                                                                    o.ReservationType.Type.Contains(searchString) ||
                                                                    o.InvoiceNumber.Contains(searchString) ||
                                                                    o.Location.Description.Contains(searchString) ||
                                                                    o.Location1.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "reqno":
                    prs = prs.OrderByDescending(o => o.RefNumber);
                    break;
                default:
                    prs = prs.OrderBy(o => o.ID);
                    break;
            }

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);

            //if (user.LocationID != 10)
            //{
            //    prs = prs.Where(x => x.LocationID == locID);
            //    return View(prs.ToPagedList(pageNumber, pageSize));
            //}
            //else
            //    return View(prs.ToPagedList(pageNumber, pageSize));

            return View(prs.ToPagedList(pageNumber, pageSize));
        }

        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult Details(int? page, int id = 0)
        {

            Session["requisitionId"] = id;

            //var pr = db.Requisitions.FirstOrDefault(r => r.ID == id && (r.RequestedBy == UserID || AcctType == 1 || AcctType == 4));
        
            var pr = entity.Requisitions.Find(id);
            if (pr == null)
            {
                return HttpNotFound();
            }
            ViewBag.isApproved = pr.ApprovalStatus;
            ViewBag.Page = page;
            return View(pr);
        }

        [AccessChecker(Action = 2, ModuleID = 9)]
        public ActionResult Create()
        {
            var pr = new Requisition();
            pr.RequestedDate = DateTime.Now;
            pr.ReqTypeID = 2;
            pr.RequisitionTypeID = 4;
            pr.RefNumber = Generator("CR");

            var employees = from s in entity.Employees
                            select new
                            {
                                ID = s.ID,
                                FullName = s.FirstName + " " + s.LastName
                            };

            var loc = entity.Locations.Where(x => x.ID != 10)
                      .Select(x => new
                      {
                          ID = x.ID,
                          Description = x.Description
                      });

            ViewBag.reqType = entity.RequisitionTypes.Where(model => model.ID == 4).ToList();
            ViewBag.PaymentStatusID = new SelectList(entity.PaymentStatus, "ID", "Status");
            ViewBag.ReqTypeID = new SelectList(entity.ReqTypes, "ID", "Type");
            ViewBag.RequisitionTypeID = new SelectList(entity.RequisitionTypes, "ID", "Type");
            ViewBag.RequestedBy = new SelectList(employees, "ID", "FullName");
            ViewBag.LocationID = new SelectList(loc, "ID", "Description");
            ViewBag.ReservationTypeID = new SelectList(entity.ReservationTypes, "ID", "Type");
            ViewBag.ShipmentTypeID = new SelectList(entity.ShipmentTypes, "ID", "Type");
            ViewBag.DropShipID = new SelectList(entity.DropShipTypes, "ID", "Type");
            ViewBag.ReservedBy = new SelectList(employees, "ID", "FullName");
            ViewBag.ValidatedBy = new SelectList(employees, "ID", "FullName");
            ViewBag.AuthorizedPerson = new SelectList(employees, "ID", "FullName");
            ViewBag.ApprovedBy = new SelectList(employees, "ID", "FullName");
            ViewBag.ApprovalStatus = new SelectList(entity.ApprovalStatus, "ID", "Status");
            ViewBag.Destination = new SelectList(loc, "ID", "Description");

            return View(pr);
        }

        [HttpPost]
        [AccessChecker(Action = 2, ModuleID = 9)]
        public ActionResult Create(Requisition req)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    req.RefNumber = CheckExistingRefNum(req.RefNumber);
                    req.ApprovalStatus = 1;
                    req.Status = false;

                    var newPR = setValue(req);
                    
                    if (req.LocationID == req.Destination)
                    {
                        ModelState.AddModelError("", "Source location should not be the same with the destination.");
                    }
                    else
                    {
                        if (newPR != null)
                        {
                            newPR.IsSync = false;

                            entity.Requisitions.Add(newPR);
                            entity.SaveChanges();

                            //return RedirectToAction("Index");

                            return RedirectToAction("Details", new { id = req.ID });
                        }
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "There's an error.");
                }
            }
            var employees = from s in entity.Employees
                            select new
                            {
                                ID = s.ID,
                                FullName = s.FirstName + " " + s.LastName
                            };

            var loc = entity.Locations.Where(x => x.ID != 10)
          .Select(x => new
          {
              ID = x.ID,
              Description = x.Description
          });

            ViewBag.PaymentStatusID = new SelectList(entity.PaymentStatus, "ID", "Status");
            ViewBag.RequestedBy = new SelectList(employees, "ID", "FullName", req.RequestedBy);
            ViewBag.LocationID = new SelectList(loc, "ID", "Description", req.LocationID);
            ViewBag.ReservationTypeID = new SelectList(entity.ReservationTypes, "ID", "Type", req.ReservationTypeID);
            ViewBag.ShipmentTypeID = new SelectList(entity.ShipmentTypes, "ID", "Type", req.ShipmentTypeID);
            ViewBag.DropShipID = new SelectList(entity.DropShipTypes, "ID", "Type", req.DropShipID);
            ViewBag.ReservedBy = new SelectList(employees, "ID", "FullName", req.ReservedBy);
            ViewBag.ValidatedBy = new SelectList(employees, "ID", "FullName", req.ValidatedBy);
             ViewBag.ApprovalStatus = new SelectList(entity.ApprovalStatus, "ID", "Status", req.ApprovalStatus);
            ViewBag.AuthorizedPerson = new SelectList(employees, "ID", "FullName", req.AuthorizedPerson);
            ViewBag.ApprovedBy = new SelectList(employees, "ID", "FullName");
            ViewBag.Destination = new SelectList(loc, "ID", "Description", req.Destination);
            return View(req);
        }

        [AccessChecker(Action = 2, ModuleID = 9)]
        public ActionResult Edit(int? id)
        {
            //var pr = db.Requisitions.FirstOrDefault(r => r.ID == id && (r.RequestedBy == UserID || AcctType == 1 || AcctType == 4));
            var pr = entity.Requisitions.FirstOrDefault(r => r.ID == id);
            if (pr.ApprovalStatus == 1)
            {
                #region DROPDOWNS
                var employees = from s in entity.Employees
                                select new
                                {
                                    ID = s.ID,
                                    FullName = s.FirstName + " " + s.LastName
                                };

                var loc = entity.Locations.Where(x => x.ID != 10)
                            .Select(x => new
                            {
                                ID = x.ID,
                                Description = x.Description
                            });

                ViewBag.PaymentStatusID = new SelectList(entity.PaymentStatus, "ID", "Status",pr.PaymentStatusID);
                ViewBag.RequestedBy = new SelectList(employees, "ID", "FullName", pr.RequestedBy);
                ViewBag.LocationID = new SelectList(loc, "ID", "Description", pr.LocationID);
                ViewBag.ReservationTypeID = new SelectList(entity.ReservationTypes, "ID", "Type", pr.ReservationTypeID);
                ViewBag.ShipmentTypeID = new SelectList(entity.ShipmentTypes, "ID", "Type", pr.ShipmentTypeID);
                ViewBag.DropShipID = new SelectList(entity.DropShipTypes, "ID", "Type", pr.DropShipID);
                ViewBag.ReservedBy = new SelectList(employees, "ID", "FullName", pr.ReservedBy);
                ViewBag.ValidatedBy = new SelectList(employees, "ID", "FullName", pr.ValidatedBy);
                ViewBag.ApprovalStatus = new SelectList(entity.ApprovalStatus, "ID", "Status", pr.ApprovalStatus);
                ViewBag.ApprovedBy = new SelectList(employees, "ID", "FullName", pr.ApprovedBy);
                ViewBag.AuthorizedPerson = new SelectList(employees, "ID", "FullName", pr.AuthorizedPerson);
                ViewBag.Destination = new SelectList(loc.Where(p => p.ID != pr.LocationID), "ID", "Description", pr.Destination);
                #endregion

                return View(pr);
            }

            return RedirectToAction("Details", new { id = id });
        }

        // POST: PR/Edit/5
        [AccessChecker(Action = 2, ModuleID = 9)]
        [HttpPost]
        public ActionResult Edit(Requisition req)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var r = db.Requisitions.FirstOrDefault(r1 => r1.ID == pr.ID && (r1.RequestedBy == UserID || AcctType == 1 || AcctType == 4)).ApprovalStatus;
                    var r = entity.Requisitions.FirstOrDefault(model => model.ID == req.ID);
                    if (r.ApprovalStatus == 1)
                    {

                        req.IsSync = false;
                        req.Status = false;
                        var newPR = setValue(req);
                        newPR.ApprovalStatus = r.ApprovalStatus;
                        entity.Entry(r).CurrentValues.SetValues(newPR);
                        entity.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Details", new { id = req.ID });
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "There's an error.");
                    throw;
                }
            }

            #region DROPDOWNS
            var employees = from s in entity.Employees
                            select new
                            {
                                ID = s.ID,
                                FullName = s.FirstName + " " + s.LastName
                            };

            var loc = entity.Locations.Where(x => x.ID != 10)
                        .Select(x => new
                        {
                          ID = x.ID,
                          Description = x.Description
                        });

            ViewBag.PaymentStatusID = new SelectList(entity.PaymentStatus, "ID", "Status",req.PaymentStatusID);
            ViewBag.RequestedBy = new SelectList(employees, "ID", "FullName", req.RequestedBy);
            ViewBag.LocationID = new SelectList(loc, "ID", "Description", req.LocationID);
            ViewBag.ReservationTypeID = new SelectList(entity.ReservationTypes, "ID", "Type", req.ReservationTypeID);
            ViewBag.ShipmentTypeID = new SelectList(entity.ShipmentTypes, "ID", "Type", req.ShipmentTypeID);
            ViewBag.DropShipID = new SelectList(entity.DropShipTypes, "ID", "Type", req.DropShipID);
            ViewBag.ReservedBy = new SelectList(employees, "ID", "FullName", req.ReservedBy);
            ViewBag.ValidatedBy = new SelectList(employees, "ID", "FullName", req.ValidatedBy);
            ViewBag.ApprovalStatus = new SelectList(entity.ApprovalStatus, "ID", "Status", req.ApprovalStatus);
            ViewBag.ApprovedBy = new SelectList(employees, "ID", "FullName", req.ApprovedBy);
            ViewBag.AuthorizedPerson = new SelectList(employees, "ID", "FullName", req.AuthorizedPerson);
            ViewBag.Destination = new SelectList(loc.Where(p => p.ID != req.LocationID), "ID", "Description", req.Destination);
            #endregion

            return View(req);
        }

        [AccessChecker(Action = 3, ModuleID = 9)]
        public ActionResult Delete(int id)
        {
            var pr = entity.Requisitions.FirstOrDefault(r => r.ID == id);
            if (pr == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (pr.ApprovalStatus == 1)
                {
                    return View(pr);
                }
            }

            return RedirectToAction("Details", new { id = pr.ID });
        }

        [AccessChecker(Action = 3, ModuleID = 9)]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            try
            {
                var pr = entity.Requisitions.Find(id);
                pr.Status = true;
                entity.Entry(pr).State = EntityState.Modified;
                entity.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
            }
            return RedirectToAction("Delete", new { id });
        }
        #endregion

        #region actions 
        [AccessChecker(Action = 5, ModuleID = 9)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            try
            {
                int approve = 0;
                // TODO: Add delete logic here
                //var pr = db.Requisitions.FirstOrDefault(r => r.ID == id && (r.RequestedBy == UserID || AcctType == 1 || AcctType == 4));
                var pr = entity.Requisitions.Find(id);

                if (pr.ApprovalStatus == 1)
                {
                    if (pr.RequisitionDetails.Count() > 0)
                    {
                        foreach (var _details in pr.RequisitionDetails)
                        {
                            if (_details.AprovalStatusID != 1)
                            {
                                approve++;
                            }
                        }

                        if (approve == pr.RequisitionDetails.Count())
                        {

                            pr.ApprovalStatus = 2;
                            pr.IsSync = false;

                            entity.Entry(pr).State = EntityState.Modified;
                            entity.SaveChanges();
                            var rd = pr.RequisitionDetails.Select(p => p.ItemID).ToList();
                            var item = entity.Items.Where(i => rd.Contains(i.ID)).Select(i => i.Code);
                                var inv = entity.Inventories.Where(i => item.Contains(i.ItemCode) && i.LocationCode == pr.LocationID).ToList();
                                if (inv != null)
                                {
                                    foreach (var _inv in inv)
                                    {
                                        var i = entity.Inventories.Find(_inv.ID);
                                        int _qty = pr.RequisitionDetails.FirstOrDefault(p => p.Item.Code == _inv.ItemCode && p.RequisitionID == id).Quantity.Value;
                                        if (pr.ReqTypeID == 2)
                                        {
                                            i.Committed = i.Committed + _qty; //invRepo.getCommited(_inv.ItemCode,pr.LocationID.Value);
                                        }
                                        else
                                        {
                                            i.Ordered = i.Ordered + _qty; //invRepo.getPurchaseOrder(_inv.ItemCode, pr.LocationID.Value);
                                        }

                                        i.InStock = invRepo.getInstocked(pr.ID, _inv.ItemCode);
                                        i.Available = (i.InStock + i.Ordered) - i.Committed;

                                        entity.Entry(i).State = EntityState.Modified;
                                        entity.SaveChanges();

                                    }
                                }
                          
                            var invitems = entity.Items.Where(i => rd.Contains(i.ID)).ToList();

                            foreach (var _item in invitems)
                            {
                                int _qty = pr.RequisitionDetails.FirstOrDefault(p => p.Item.Code == _item.Code && p.RequisitionID == id).Quantity.Value;
                                var inv1 = entity.Inventories.Where(i => i.ItemCode == _item.Code && i.LocationCode == pr.LocationID).ToList();
                                if (inv1.Count == 0)
                                {
                                    Inventory inventory = new Inventory();
                                    inventory.Year = _item.Year;
                                    inventory.ItemCode = _item.Code;
                                    inventory.POSBarCode = _item.Barcode;
                                    inventory.Description = _item.DescriptionPurchase;
                                    inventory.Category = _item.Category.Description;
                                    inventory.InventoryUoM = ""; //_item.UnitOfMeasurement.Description
                                    inventory.InventoryStatus = 2;
                                    inventory.LocationCode = pr.LocationID;
                                    if (pr.ReqTypeID == 2)
                                    {
                                        inventory.Committed = _qty;
                                        inventory.Ordered = 0;    //invRepo.getCommited(_inv.ItemCode,pr.LocationID.Value);
                                    }
                                    else
                                    {
                                        inventory.Ordered = _qty;
                                        inventory.Committed = 0;   //invRepo.getPurchaseOrder(_inv.ItemCode, pr.LocationID.Value);
                                    }
                                    inventory.InStock = 0;
                                    inventory.Available = (inventory.InStock + inventory.Ordered) - inventory.Committed;


                                    entity.Inventories.Add(inventory);
                                    entity.SaveChanges();
                                }
                            }


                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Not all items are approved");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "There's no item");
                    }
                }
            }
            catch (Exception msg)
            {
                msg.Message.ToString();

            }
            return View();
        }

        // POST: PR/Approve/5
        [AccessChecker(Action = 5, ModuleID = 9)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revise(int id)
        {
            try
            {
                // TODO: Add delete logic here
                //var pr = db.Requisitions.FirstOrDefault(r => r.ID == id && (r.RequestedBy == UserID || AcctType == 1 || AcctType == 4));
                var pr = entity.Requisitions.Find(id);

                if (pr.RequisitionDetails.Count() > 0)
                {
                    pr.ApprovalStatus = 4;
                    pr.IsSync = false;

                    entity.Entry(pr).State = EntityState.Modified;
                    entity.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There's no item");
                }
            }
            catch
            {
            }
            return View();
        }

        // POST: PR/Denied/5
        [AccessChecker(Action = 5, ModuleID = 9)]
        [HttpPost]
        public ActionResult Denied(int id)
        {
            try
            {
                // TODO: Add delete logic here
                var pr = entity.Requisitions.Find(id);
                pr.ApprovalStatus = 3;
                pr.IsSync = false;

                entity.Entry(pr).State = EntityState.Modified;
                entity.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PR/Items/5
        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult Items(int id, int? page)
        {
            var items = entity.RequisitionDetails
                        .ToList()
                        .FindAll(rd => rd.RequisitionID == id);

            ViewBag.PRid = id;

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: PR/ApprovedItems/5
        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult ApprovedItems(int id, int? page)
        {
            var items = entity.RequisitionDetails
                        .ToList()
                        .FindAll(rd => rd.RequisitionID == id && rd.AprovalStatusID == 2);

            ViewBag.PRid = id;

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: PR/PendingItems/5
        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult PendingItems(int id, int? page)
        {
            int UserID = Convert.ToInt32(Session["sessionuid"]);
            int UserType = Convert.ToInt32(Session["usertype"]);

            var requisition = entity.Requisitions.FirstOrDefault(r => r.ID == id);
            //if (requisition.RequestedBy != UserID && UserType != 1 && UserType != 4)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var items = entity.RequisitionDetails
                        .ToList()
                        .FindAll(rd => rd.RequisitionID == id && rd.AprovalStatusID == 1 );

            var reqdetails = entity.RequisitionDetails.FirstOrDefault(p => p.RequisitionID == id && p.AprovalStatusID == 2);
            //var items = db.RequisitionDetails
            //            .ToList()
            //            .FindAll(rd => rd.RequisitionID == id && rd.AprovalStatusID == 1 && rd.Requisition.RequestedBy == UserID);

            ViewBag.PRid = id;
            try
            {
                ViewBag.Approved = reqdetails.AprovalStatusID.ToString();
            }
            catch { ViewBag.Approved = 1; }
           // ViewBag.RequestedBy =
            ViewBag.UserID = UserID;
            ViewBag.AcctType = UserType;
            ViewBag.IsApproved = requisition.ApprovalStatus;

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return PartialView(items.ToPagedList(pageNumber, pageSize));
        }

        // GET: PR/DeniedItems/5
        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult DeniedItems(int id, int? page)
        {
            var items = entity.RequisitionDetails
                        .ToList()
                        .FindAll(rd => rd.RequisitionID == id && rd.AprovalStatusID == 3);

            ViewBag.PRid = id;

            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);
            int pageNumber = (page ?? 1);
            return View(items.ToPagedList(pageNumber, pageSize));
        }

        // POST: PR/DenyItem/5
        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult DenyItem(int id, int itemID)
        {
            try
            {
                var item = entity.RequisitionDetails.Find(itemID);
                if (item != null)
                {
                    item.AprovalStatusID = 3;
                    item.IsSync = false;

                    entity.Entry(item).State = EntityState.Modified;
                    entity.SaveChanges();
                }
            }
            catch
            {

            }
            return RedirectToAction("PendingItems", "Customer", new { id = id });
        }

        // GET: PR/DenyItemPartial/5
        public ActionResult DenyItemPartial(int id)
        {
            ViewBag.PRid = id;

            return PartialView();
        }

        // POST: PR/DenyItemPartial/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DenyItemPartial(int id, string Reason)
        {
            try
            {
                if (!String.IsNullOrEmpty(Reason))
                {
                    var req = entity.Requisitions.Find(id);
                    req.ApprovalStatus = 3;
                    req.Remarks = Reason;
                    entity.Entry(req).State = EntityState.Modified;

                    entity.SaveChanges();

                    var reqdetails = entity.RequisitionDetails.Where(i => i.RequisitionID == id).ToList();
                    if (reqdetails != null)
                    {
                        foreach (var _rd in reqdetails)
                        {
                            var rd = entity.RequisitionDetails.Find(_rd.ID);
                            rd.AprovalStatusID = 3;

                            entity.Entry(rd).State = EntityState.Modified;
                            entity.SaveChanges();
                        }

                    }


               
                }
                else
                {
                    TempData["Error"] = "Reason is required";
                }
            }
            catch { TempData["Error"] = "There's an error"; }

            return RedirectToAction("Index");
        }

        [AccessChecker(Action = 1, ModuleID = 9)]
        public ActionResult ApproveItem(int id, int itemID, int reqID)
        {
            try
            {
                var req = entity.RequisitionDetails.Find(reqID);
                var itm = entity.Items.FirstOrDefault(x => x.ID == itemID);
                var inventory = entity.Inventories.FirstOrDefault(x => x.Description == itm.Description);

                var quantity = req.Quantity;

                if (req != null)
                {
                    req.AprovalStatusID = 2;
                    req.IsSync = false;
                    if (req.Requisition.ReqTypeID != 2)
                    {
                        req.Ordered = Convert.ToInt32(getPurchaseOrder(req.ItemID) + req.Ordered);
                    }
                    req.Committed = reqDetailRepo.getCommited(req.RequisitionID, itemID) + req.Quantity;
                    int avail = (Convert.ToInt32(req.InStock) + Convert.ToInt32(req.Ordered)) - Convert.ToInt32(req.Committed);
                    req.Available = avail;
                    //req.InStock -= req.Quantity;
                    entity.Entry(req).State = EntityState.Modified;
                    //db.SaveChanges();
                }
                //if (inventory != null)
                //{
                //    if (req.Requisition.ReqTypeID == 1)
                //    {
                //        inventory.Ordered = Convert.ToInt32(getPurchaseOrder(req.ItemID) + req.Ordered);
                //    }
                //    inventory.Committed = inventory.Committed.Value + quantity;
                //    entity.Entry(inventory).State = EntityState.Modified;
                //}
                entity.SaveChanges();
            }
            catch
            {

            }
            return RedirectToAction("PendingItems", "Reservation", new { id = id });
        }

        // GET: PR/AddItemPartial/5
        public ActionResult AddItemPartial(int id)
        {
            int approved = entity.Requisitions.Find(id).ApprovalStatus.Value;
           
                ViewBag.PRid = id;
                ViewBag.ItemID = new SelectList(entity.Items, "ID", "Description");

                return PartialView();
            }
        
        // POST: PR/AddItemPartial/5
        [HttpPost]
        public ActionResult AddItemPartial(int id, RequisitionDetail rd)
        {
            var itm = entity.Items.FirstOrDefault(x => x.ID == rd.ItemID);
            var itmID = rd.ItemID;
            var desc = itm.Description;

            var rq = entity.Requisitions.Find(id);
            try
            {
                // TODO: Add insert logic here
                rd.RequisitionID = id;
                rd.AprovalStatusID = 1; //submitted
                rd.ReferenceQuantity = rd.Quantity;
                rd.Committed = reqDetailRepo.getCommited(id, itmID);

                rd.Ordered = reqDetailRepo.getPurchaseOrder(rq.LocationID.Value,itmID);

                rd.InStock = reqDetailRepo.getInstocked(id, desc);

                rd.Available = rd.InStock + rd.Ordered - rd.Committed;
                var rd1 = entity.RequisitionDetails.Where(r => r.RequisitionID == rd.RequisitionID && r.ItemID == rd.ItemID).ToList();

                if (rd1.Count() > 0)
                {
                    TempData["PartialError"] = "Item is already in the list.";
                }
                else
                {
                    rd.IsSync = false;
                    entity.RequisitionDetails.Add(rd);
                    entity.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                TempData["PartialError"] = "There's an error.";
            }

            //ViewBag.PRid = id;
            //ViewBag.ItemID = new SelectList(db.Items, "ID", "Description", rd.ItemID);
            //ViewBag.AprovalStatusID = new SelectList(db.ApprovalStatus, "ID", "Status", rd.AprovalStatusID);

            //return RedirectToAction("PendingItems", new { id = id });
            return RedirectToAction("Details", new { id = id });
        }

        // GET: PR/EditItemPartial/5
        public ActionResult EditItemPartial(int id)
        {
            var rd = entity.RequisitionDetails.Find(id);

            ViewBag.AprovalStatusID = new SelectList(entity.ApprovalStatus, "ID", "Status", rd.AprovalStatusID);
            ViewBag.ItemID = new SelectList(entity.Items, "ID", "Description", rd.ItemID);

            return PartialView(rd);
        }

        // POST: PR/EditItemPartial/5
        [HttpPost]
        public ActionResult EditItemPartial(int id,RequisitionDetail rd)
        {
            try
            {
                var prq = entity.RequisitionDetails.Find(rd.ID);
            
                var itm = entity.Items.Find(rd.ItemID);
                if (prq.ItemID != rd.ItemID || prq.Quantity != rd.Quantity)
                {
                    rd.PreviousItemID = prq.ItemID;
                    rd.PreviousQuantity = prq.Quantity;
             
                    rd.Committed = reqDetailRepo.getCommited(id, rd.ItemID);
                    rd.Ordered = reqDetailRepo.getPurchaseOrder(prq.Requisition.LocationID.Value, rd.ItemID);
                    rd.InStock = reqDetailRepo.getInstocked(id, itm.Description);

                    rd.Available = rd.InStock + rd.Ordered - rd.Committed;
                    rd.Remarks = rd.Remarks;
                }
                else
                {
                    rd.Committed = reqDetailRepo.getCommited(id, rd.ItemID);
                    rd.Ordered = reqDetailRepo.getPurchaseOrder(prq.Requisition.LocationID.Value, rd.ItemID);
                    rd.InStock = reqDetailRepo.getInstocked(id, itm.Description);

                    rd.Available = rd.InStock + rd.Ordered - rd.Committed;
                    rd.Remarks = rd.Remarks;
                    rd.PreviousQuantity = prq.Quantity;
                    rd.PreviousItemID = prq.PreviousItemID; 
                }

                prq.ItemID = rd.ItemID;
                prq.Quantity = rd.Quantity;
                prq.ReferenceQuantity = rd.Quantity;
                prq.InStock = rd.InStock;
                prq.Ordered = rd.Ordered;
                prq.Committed = rd.Committed;
                prq.Available = rd.Available;
                prq.Remarks = rd.Remarks;
                prq.PreviousItemID = rd.PreviousItemID;
                prq.PreviousQuantity = rd.PreviousQuantity;

                prq.IsSync = false;
                entity.Entry(prq).CurrentValues.SetValues(rd);
                entity.SaveChanges();

                var _res = entity.RequisitionDetails.Find(rd.ID);
                _res.ReferenceQuantity = rd.Quantity;
                entity.SaveChanges();
            }
            catch
            {
                //throw;
                TempData["PartialError"] = "There's an error.";
            }

            if (rd.AprovalStatusID == 1)
            {       
                return RedirectToAction("Details", new { id = rd.RequisitionID });
            }
            return RedirectToAction("ApprovedItems", new { id = rd.RequisitionID });
        }

        // GET: PR/DeleteItemPartial/5
        public ActionResult DeleteItemPartial(int id)
        {
            var rd = entity.RequisitionDetails.Find(id);

            return PartialView(rd);
        }

        // POST: PR/DeleteItemPartial/5
        [HttpPost, ActionName("DeleteItemPartial")]
        public ActionResult DeleteItemPartialConfirm(int id)
        {
            var rd = entity.RequisitionDetails.Find(id);

            int? reqID = rd.RequisitionID;
            try
            {
                entity.RequisitionDetails.Remove(rd);
                entity.SaveChanges();
            }
            catch
            {
                TempData["PartialError"] = "There's an error.";
            }

            return RedirectToAction("Details", new { id = reqID });
        }
        #endregion

    }
}