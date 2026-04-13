using Microsoft.AspNetCore.Mvc;

[Route("Menu")]
public class MenuController : Controller
{
    [HttpGet("GetSubMenu")]
    public IActionResult GetSubMenu(string menuType)
    {
        string menuHtml = "";

        switch (menuType)
        {
            //case "admin-dashboard":
            //    return PartialView("~/Views/Shared/Menu/_AdminDashboardSubmenu.cshtml");
            case "admin-users":
                menuHtml = "<a href='/Admin/List'>Manage Admins</a>  <a href='/Staff/List'>Manage Staff</a>  <a href='/Coach/List'>Manage Coaches</a>  <a href='/Child/List'>Manage Children</a>";
                break;
            case "admin-reports":
                menuHtml = "<a href='/Admin/List'>Manage Admins</a> | <a href='/Staff/List'>Manage Staff</a> | <a href='/Coach/List'>Manage Coaches</a> | <a href='/Child/List'>Manage Children</a>";
                break;
            // Add cases for Staff, Coach, and Child submenus
            default:
                menuHtml = ""; // Empty response if no submenu found
                break;

        }
        return Content(menuHtml, "text/html");
    }
}
