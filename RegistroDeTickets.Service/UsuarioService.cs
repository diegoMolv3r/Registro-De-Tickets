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

        void EliminarUsuario(Usuario usuario);

        Usuario ObtenerUsuarioPorId (int id);

        Usuario BuscarUsuarioPorEmail(string email);

        void DesignarUsuarioComoTecnico(Usuario usuario);

        void DesignarUsuarioComoCliente(Usuario usuario);

        List<Usuario> ObtenerTecnicos();
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

        public List<Usuario> ObtenerTecnicos()
        {
            return _usuarioRepository.ObtenerTecnicos();
        }
    }
}
