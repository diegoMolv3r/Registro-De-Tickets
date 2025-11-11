using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Repository;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace RegistroDeTickets.Service
{
    public interface IUsuarioService
    {
        // CREATE
        void AgregarUsuario(Usuario usuario);

        // READ
        List<Usuario> ObtenerUsuarios();

        // UPDATE
        void EditarUsuario(Usuario usuario);

        // DELETE
        void EliminarUsuario(Usuario usuario);

        // Buscar por email
        Usuario BuscarPorEmail(string email);

        //Usuario BuscarUsuarioPorEmail(string email);

        string? GenerarTokenRecuperacion(string email);

        bool RestablecerContrasenia(string email, string token, string nuevaContrasenia);

        void DesignarUsuarioComoTecnico(Usuario usuario);


        List<Usuario> ObtenerTecnicos();

        Usuario ObtenerUsuarioPorId(int id);
        //Usuario RegistrarUsuarioGoogle(string email, string nombreCompleto);

        string renombrarUsuarioGoogle(string email, string nombreCompleto);

    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public void AgregarUsuario(Usuario usuario)
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, usuario.PasswordHash);

            _usuarioRepository.AgregarUsuario(usuario);
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return _usuarioRepository.ObtenerUsuarios();
        }

        public void EditarUsuario(Usuario usuario)
        {
            _usuarioRepository.EditarUsuario(usuario);
        }

        public string? GenerarTokenRecuperacion(string email)
        {
            var usuario = _usuarioRepository.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                return null;
            }

            string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            usuario.TokenHashRecuperacion = HashearToken(token);

            usuario.TokenHashRecuperacionExpiracion = DateTime.UtcNow.AddMinutes(30);

            _usuarioRepository.EditarUsuario(usuario);

            return token;
        }

        public bool RestablecerContrasenia(string email, string token, string nuevaContrasenia)
        {
            var usuario = _usuarioRepository.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                return false;
            }

            var hashTokenRecibido = HashearToken(token);
            if (string.IsNullOrEmpty(usuario.TokenHashRecuperacion) || usuario.TokenHashRecuperacion != hashTokenRecibido)
            {
                return false;
            }

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, nuevaContrasenia);

            _usuarioRepository.EditarUsuario(usuario);

            return true;
        }

        private string HashearToken(string token)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
                return Convert.ToHexString(bytes);
            }
        }

        public void EliminarUsuario(Usuario usuario)
        {
            _usuarioRepository.EliminarUsuario(usuario);
        }

        public Usuario BuscarPorEmail(string email)
        {
            return _usuarioRepository.BuscarPorEmail(email);
        }
        /*
        public Usuario RegistrarUsuarioGoogle(string email, string nombreCompleto)
        {
            var usuarioExistente = _usuarioRepository.BuscarPorEmail(email);
            if (usuarioExistente != null)
            {
                return usuarioExistente;
            }

            //trata de setear e primer nombre con username si no puede pone el mail

            string primerNombre = (nombreCompleto ?? email).Split(' ')[0];

            var nuevoUsuario = new Usuario
            {
                UserName = primerNombre,
                Email = email,
                PasswordHash = "", // Google gestiona la autenticación
                Estado = "Activo"
            };

            _usuarioRepository.AgregarUsuario(nuevoUsuario);
            return nuevoUsuario;
        }*/

        public string renombrarUsuarioGoogle(string email, string nombreCompleto)
        {
           
            return (nombreCompleto ?? email).Split(' ')[0]; ;
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return _usuarioRepository.ObtenerUsuarioPorId(id);
        }

        public void DesignarUsuarioComoTecnico(Usuario usuario)
        {
            if (usuario.Tecnico == null)
            {
                usuario.Tecnico = new Tecnico { IdNavigation = usuario };
                _usuarioRepository.AgregarTecnico(usuario.Tecnico);
                _usuarioRepository.EditarUsuario(usuario);
            }
        }

     

        public List<Usuario> ObtenerTecnicos()
        {
            return _usuarioRepository.ObtenerTecnicos();
        }
    }
}
