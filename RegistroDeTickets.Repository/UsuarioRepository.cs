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

        // UPDATE --> seria necesario implementar este metodo?
        void EditarUsuario(Usuario usuario);

        // DELETE
        void EliminarUsuario(Usuario usuario);

        Usuario ObtenerUsuarioPorId(int id);

        void AgregarTecnico(Tecnico tecnico);

        void AgregarCliente(Cliente cliente);

        Usuario BuscarUsuarioPorEmail(string email);

        List<Usuario> ObtenerTecnicos();
    }
    public class UsuarioRepository(RegistroDeTicketsPw3Context ctx) : IUsuarioRepository
    {
        private readonly RegistroDeTicketsPw3Context _ctx = ctx;

        public void AgregarUsuario(Usuario usuario)
        {
            _ctx.Usuarios.Add(usuario);
            _ctx.SaveChanges();
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return _ctx.Usuarios
        .Include(u => u.Administrador)
        .Include(u => u.Tecnico)
        .Include(u => u.Cliente)
        .ToList();
        }

        public void EditarUsuario(Usuario usuario)
        {
            _ctx.Usuarios.Update(usuario);
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
            _ctx.Usuarios.Remove(usuario);
            _ctx.SaveChanges();
        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            return _ctx.Usuarios.FirstOrDefault(u => u.Email == email);
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            return _ctx.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public List<Usuario> ObtenerTecnicos()
        { 
            return _ctx.Usuarios
                .Include(u => u.Tecnico)
                .Where(u => u.Tecnico != null)
                .ToList();
        }
    }
}