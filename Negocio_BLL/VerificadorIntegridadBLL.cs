using Acceso_DAL;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_BLL
{
    public class VerificadorIntegridadBLL
    {
        MP_VerificadorIntegridad verIntegridad = new MP_VerificadorIntegridad();

        public List<string> ExtraerDVH()
        {
            return verIntegridad.ExtraerDVH("Usuarios");
        }

        public void ActualizarDVV()
        {
            verIntegridad.ActualizarDVV(VerificadorIntegridad.CalcularDVV(ExtraerDVH()),"Usuarios");
        }
    }
}
