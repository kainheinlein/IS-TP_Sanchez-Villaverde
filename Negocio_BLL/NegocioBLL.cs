using Acceso_DAL;
using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_BLL
{
    public class NegocioBLL
    {
        private MP_Usuario mpUsuario = new MP_Usuario();

        public List<UsuarioBE> ListarUsuarios()
        {
            return mpUsuario.ListarUsuarios();
        }
    }
}
