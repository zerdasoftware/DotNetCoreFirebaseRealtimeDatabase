using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using DotNetCoreFirebaseRealtimeDatabase.Models;

namespace DotNetCoreFirebaseRealtimeDatabase.Controllers
{
    public class EmployeeController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "EhYavvhhnarP8qAD9R7e4mPKC1NZ1jZxgTvmnpsb",
            BasePath = "https://fir-login-ce5d8-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Employee");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Employee>();
            foreach (var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Employee>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            try
            {
                AddEmployeeToFirebase(employee);
                ModelState.AddModelError(String.Empty, "Added Succesfuly");
            }
            catch (Exception ex)
            {
                throw;
                ModelState.AddModelError(String.Empty, ex.Message);
            }

            return RedirectToAction("Index");
        }

        private void AddEmployeeToFirebase(Employee employee)
        {
            client = new FireSharp.FirebaseClient(config);
            var data = employee;
            PushResponse response = client.Push("Employee/", data);
            data.id = response.Result.name;
            SetResponse setresponse = client.Set("Employee/" + data.id, data);
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Employee/+id");
            Employee data = JsonConvert.DeserializeObject<Employee>(response.Body);
            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Employee/+id");
            Employee data = JsonConvert.DeserializeObject<Employee>(response.Body);
            return View(data);
        }
    }
}
