using eCode.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCode.Controllers
{
    public class ContentController : Controller
    {

        #region Eventos GET

        public IActionResult CarregarDesafios()
        {
            API api = new API();
            string filtro = string.IsNullOrEmpty(Request.Query["filtro"]) ? string.Empty : Request.Query["filtro"];
            string? apoiador = Request.Cookies["Apoiador"] != null ? Request.Cookies["Apoiador"] : "N";
            string? perfil = Request.Cookies["Perfil"] != null ? Request.Cookies["Perfil"] : "R";

            return View("Desafios", api.ListarDesafios(filtro, apoiador, perfil));
        }

        public IActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
