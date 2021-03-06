﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if(id == null)
            {
                return View(coverType);
            }
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            coverType = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CovetType_Get, parameter);
   
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);
                if (coverType.Id ==0)
                {
                    _unitOfWork.SP_Call.Execute(SD.Proc_CovetType_Create,parameter);
                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _unitOfWork.SP_Call.Execute(SD.Proc_CovetType_Update, parameter);

                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.SP_Call.List<CoverType>(SD.Proc_CovetType_GetAll,null);
            return Json(new { data = allObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);

            var objFromDb = _unitOfWork.SP_Call.OneRecord<CoverType>(SD.Proc_CovetType_Get, parameter);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.SP_Call.Execute(SD.Proc_CovetType_Delete,parameter);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
