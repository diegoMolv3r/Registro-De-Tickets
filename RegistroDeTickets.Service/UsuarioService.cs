using RegistroDeTickets.Data.Entidades;
using RegistroDeTickets.Repository;

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

        void DesignarUsuarioComoTecnico(Usuario usuario);

        void DesignarUsuarioComoCliente(Usuario usuario);

        List<Usuario> ObtenerTecnicos();

        Usuario ObtenerUsuarioPorId(int id);
        Usuario RegistrarUsuarioGoogle(string email, string nombreCompleto);

    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public void AgregarUsuario(Usuario usuario)
        {
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

        public void EliminarUsuario(Usuario usuario)
        {
            _usuarioRepository.EliminarUsuario(usuario);
        }

        public Usuario BuscarPorEmail(string email)
        {
            return _usuarioRepository.BuscarPorEmail(email);
        }

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
                Username = primerNombre,
                Email = email,
                PasswordHash = "" // Google gestiona la autenticación
            };

            _usuarioRepository.AgregarUsuario(nuevoUsuario);
            return nuevoUsuario;
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

        public void DesignarUsuarioComoCliente(Usuario usuario)
        {
            if (usuario.Cliente == null)
            {
                usuario.Cliente = new Cliente { IdNavigation = usuario };
                _usuarioRepository.AgregarCliente(usuario.Cliente);
                _usuarioRepository.EditarUsuario(usuario);
            }
        }

        public List<Usuario> ObtenerTecnicos()
        {
            return _usuarioRepository.ObtenerTecnicos();
        }
    }
}
