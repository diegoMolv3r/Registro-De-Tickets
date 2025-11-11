using Microsoft.EntityFrameworkCore;
using RegistroDeTickets.Data.Entidades;

namespace RegistroDeTickets.Repository
{
    public interface IUsuarioRepository
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

        Usuario ObtenerUsuarioPorId(int id);

        void AgregarTecnico(Tecnico tecnico);

        void AgregarCliente(Cliente cliente);

        Usuario BuscarUsuarioPorEmail(string email);

        List<Usuario> ObtenerTecnicos();

        List<Usuario> ObtenerTecnicosInactivos();

        List<Usuario> ObtenerUsuariosInactivos();
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly RegistroDeTicketsPw3Context _ctx;

        public UsuarioRepository(RegistroDeTicketsPw3Context ctx)
        {
            _ctx = ctx;
        }

        public void AgregarUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            var usuarioDuplicado = BuscarUsuarioPorEmail(usuario.Email);

            if (usuarioDuplicado != null)
            {
                throw new InvalidOperationException("Ya existe un usuario con este correo.");
            }

            _ctx.Users.Add(usuario);
            _ctx.SaveChanges();
        }


        public List<Usuario> ObtenerUsuarios()
        {
            return _ctx.Users
        .Include(u => u.Administrador)
        .Include(u => u.Tecnico)
        .Include(u => u.Cliente)
        .Where(u => u.Estado == "Activo")
        .ToList();
        }

        public void EditarUsuario(Usuario usuario)
        {
            _ctx.Users.Update(usuario);
            _ctx.SaveChanges();
        }

        public void AgregarTecnico(Tecnico tecnico)
        {
            _ctx.Tecnicos.Add(tecnico);
            _ctx.SaveChanges();
        }

        public void AgregarCliente(Cliente cliente)
        {
            _ctx.Clientes.Add(cliente);
            _ctx.SaveChanges();
        }

        public void EliminarUsuario(Usuario usuario)
        {
            var usuarioEncontrado = _ctx.Users.Find(usuario.Id);
            if (usuarioEncontrado != null)
            {
                usuarioEncontrado.Estado = "Inactivo";
                _ctx.SaveChanges();
            }


        }

        public Usuario BuscarPorEmail(string email)
        {
            return _ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            return _ctx.Users.FirstOrDefault(u => u.Email == email);
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return _ctx.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<Usuario> ObtenerTecnicos()
        {
            return _ctx.Users
            .Include(u => u.Tecnico)
            .Where(u => u.Tecnico != null && u.Estado == "Activo")
            .ToList();
        }

        // PARA OBTENER LOS INACTIVOS SI ES NECESARIO

        public List<Usuario> ObtenerUsuariosInactivos()
        {
            return _ctx.Users
                .Include(u => u.Administrador)
                .Include(u => u.Tecnico)
                .Include(u => u.Cliente)
                .Where(u => u.Estado == "Inactivo") 
                .ToList();
        }

        public List<Usuario> ObtenerTecnicosInactivos()
        {
            return _ctx.Users
                .Include(u => u.Tecnico)
                .Where(u => u.Tecnico != null && u.Estado == "Inactivo") 
                .ToList();
        }
    }
}
