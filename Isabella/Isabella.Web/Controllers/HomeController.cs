﻿namespace Isabella.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Isabella.Web.Models;
    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.ProductViewModel;
    using Isabella.Web.ViewModels.AggregateViewModel;

    /// <summary>
    /// Pagina de inicio.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductServiceController _productServiceController;
        private readonly AggregateServiceController _aggregateServiceController;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="productServiceController"></param>
        /// <param name="aggregateServiceController"></param>
        public HomeController(ILogger<HomeController> logger, 
        ProductServiceController productServiceController, AggregateServiceController aggregateServiceController)
        {
            this._productServiceController = productServiceController;
            this._aggregateServiceController = aggregateServiceController;
            _logger = logger;
        }

        /// <summary>
        /// Privacy
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Muestra la View del Dashboard
        /// </summary>
        /// <returns></returns>
        public IActionResult Dashboard()
        {
            return View();
        }

        /// <summary>
        /// NotFound
        /// </summary>
        /// <returns></returns>
        public IActionResult _NotFound()
        {
            return View();
        }

        /// <summary>
        /// Excepción
        /// </summary>
        /// <returns></returns>
        public IActionResult _ServerError()
        {
            return View();
        }

        /// <summary>
        /// No autorizado.
        /// </summary>
        /// <returns></returns>
        public IActionResult _NotAuthorized()
        {
            return View();
        }
        
        /// <summary>
        /// Muestra la View para el manejo de errores.
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
