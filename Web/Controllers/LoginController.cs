﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Headers;
using Web.Entidades;
using Web.Models;

namespace Web.Controllers;

public class LoginController : ControllerGenerico
{
    public LoginController(IConfiguration configuration) : base(configuration)
    {
    }

    public IActionResult Index()
    {
        return View(new Usuario());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Index(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return View(usuario);
        }

        HttpResponseMessage response = await _client.PostAsJsonAsync("Auth/login", usuario);
        if (response.IsSuccessStatusCode)
        {
            //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
            string? token = await response.Content.ReadFromJsonAsync<string>();
            if (token != null)
            {
                Response.Cookies.Append("jwt", token, _cookieOptions);
            }
            return RedirectToAction("Index", "Home");
        }
        TempData["error"] = "El usuario no existe o clave incorrecta";
        return View(usuario);
    }
}