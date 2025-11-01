using Microsoft.AspNetCore.Identity;
using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Repository;
using System.Text;
using System.Security.Cryptography;

namespace RegistroDeTickets.Service
{   
    public interface IUsuarioService
    {
        // CREATE
        void AgregarUsuario(Usuario usuario);
        // READ
        List<Usuario> ObtenerUsuarios();

        void EliminarUsuario(Usuario usuario);

        Usuario ObtenerUsuarioPorId (int id);

        Usuario BuscarUsuarioPorEmail(string email);

        string? GenerarTokenRecuperacion(string email);

        bool RestablecerContrasenia(string email, string token, string nuevaContrasenia);

        void DesignarUsuarioComoTecnico(Usuario usuario);

        void DesignarUsuarioComoCliente(Usuario usuario);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        private readonly IPasswordHasher<Usuario> _passwordHasher;

        //private Usuario usuario;


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

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            return _usuarioRepository.BuscarUsuarioPorEmail(email);
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return _usuarioRepository.ObtenerUsuarioPorId(id);
        }

        public void DesignarUsuarioComoTecnico(Usuario usuario) { 
            if (usuario.Tecnico == null) {
                usuario.Tecnico = new Tecnico { IdNavigation = usuario };
                _usuarioRepository.AgregarTecnico(usuario.Tecnico);
                _usuarioRepository.EditarUsuario(usuario);
            }
        }

        public void DesignarUsuarioComoCliente(Usuario usuario)
        {
            if (usuario.Cliente == null)
            {
                usuario.Cliente = new Cliente { IdNavigation = usuario };
                _usuarioRepository.AgregarCliente(usuario.Cliente);
                _usuarioRepository.EditarUsuario(usuario);
            }
        }

    }
}
