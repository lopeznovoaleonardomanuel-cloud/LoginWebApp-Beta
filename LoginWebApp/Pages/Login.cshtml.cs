using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace LoginWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string _connectionString;

        public LoginModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CadenaSQL");
        }

        [BindProperty]
        public string Correo { get; set; }

        [BindProperty]
        public string Clave { get; set; }

        public string Mensaje { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    
                    string sql = "SELECT Correo FROM Usuarios WHERE Correo = @correo AND Clave = @clave";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@correo", Correo);
                    command.Parameters.AddWithValue("@clave", Clave);

                    connection.Open();
                   
                    var usuarioLogueado = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(usuarioLogueado))
                    {
                        
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, usuarioLogueado),
                            new Claim(ClaimTypes.Email, usuarioLogueado)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity)
                        );

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        Mensaje = "Usuario o contraseña no válidos.";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error de conexión: " + ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }
    }
}