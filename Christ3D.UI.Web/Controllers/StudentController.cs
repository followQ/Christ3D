﻿using System;
using Christ3D.Application.Interfaces;
using Christ3D.Application.ViewModels;
using Christ3D.Domain.Core.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Christ3D.UI.Web.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentAppService _studentAppService;
        IValidator<StudentViewModel> _validator;
        private IMemoryCache _cache;
        // 将领域通知处理程序注入Controller
        private readonly DomainNotificationHandler _notifications;

        public StudentController(IStudentAppService studentAppService, IMemoryCache cache, INotificationHandler<DomainNotification> notifications)
        {
            _studentAppService = studentAppService;
            _cache = cache;
            // 强类型转换
            _notifications = (DomainNotificationHandler)notifications;
        }

        // GET: Student
        public ActionResult Index()
        {
            return View(_studentAppService.GetAll());
        }

        // GET: Student/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentViewModel = _studentAppService.GetById(id.Value);

            if (studentViewModel == null)
            {
                return NotFound();
            }

            return View(studentViewModel);
        }

        // GET: Student/Create
        // 页面
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // 方法
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(StudentViewModel studentViewModel)
        {
            try
            {
                //_cache.Remove("ErrorData");
                //ViewBag.ErrorData = null;
                // 视图模型验证
                if (!ModelState.IsValid)
                    return View(studentViewModel);

                #region 删除命令验证
                ////添加命令验证
                //RegisterStudentCommand registerStudentCommand = new RegisterStudentCommand(studentViewModel.Name, studentViewModel.Email, studentViewModel.BirthDate, studentViewModel.Phone);

                ////如果命令无效，证明有错误
                //if (!registerStudentCommand.IsValid())
                //{
                //    List<string> errorInfo = new List<string>();
                //    //获取到错误，请思考这个Result从哪里来的 
                //    foreach (var error in registerStudentCommand.ValidationResult.Errors)
                //    {
                //        errorInfo.Add(error.ErrorMessage);
                //    }
                //    //对错误进行记录，还需要抛给前台
                //    ViewBag.ErrorData = errorInfo;
                //    return View(studentViewModel);
                //} 
                #endregion

                // 执行添加方法
                _studentAppService.Register(studentViewModel);

                //var errorData = _cache.Get("ErrorData");
                //if (errorData == null)

                // 是否存在消息通知
                if (!_notifications.HasNotifications())
                    ViewBag.Sucesso = "Student Registered!";

                return View(studentViewModel);
            }
            catch (Exception e)
            {
                return View(e.Message);
            }
        }

        // GET: Student/Edit/5
        [HttpGet]
        [Authorize(Policy = "CanWriteStudentData")]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentViewModel = _studentAppService.GetById(id.Value);

            if (studentViewModel == null)
            {
                return NotFound();
            }

            return View(studentViewModel);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanWriteStudentData")]
        public IActionResult Edit(StudentViewModel studentViewModel)
        {
            if (!ModelState.IsValid) return View(studentViewModel);

            _studentAppService.Update(studentViewModel);

            if (!_notifications.HasNotifications())
                ViewBag.Sucesso = "Student Updated!";

            return View(studentViewModel);
        }

        // GET: Student/Delete/5
        [Authorize(Policy = "CanWriteOrRemoveStudentData")]
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentViewModel = _studentAppService.GetById(id.Value);

            if (studentViewModel == null)
            {
                return NotFound();
            }

            return View(studentViewModel);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanWriteOrRemoveStudentData")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _studentAppService.Remove(id);

            if (!_notifications.HasNotifications())
                return View(_studentAppService.GetById(id));

            ViewBag.Sucesso = "Student Removed!";
            return RedirectToAction("Index");
        }

        [Route("history/{id:guid}")]
        public JsonResult History(Guid id)
        {
            var studentHistoryData = _studentAppService.GetAllHistory(id);
            return Json(studentHistoryData);
        }
    }
}