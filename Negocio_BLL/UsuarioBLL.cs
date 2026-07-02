using Acceso_DAL;
using Entidad_BE;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio_BLL
{
    public class UsuarioBLL
    {
        public int maxIntentos = 3;
        private UsuarioBE usuario;
        private MP_Usuario mpUsuario = new MP_Usuario();
        private BitacoraBLL bitacora = new BitacoraBLL();
        private VerificadorIntegridadBLL integridad = new VerificadorIntegridadBLL();

        public List<UsuarioBE> ListarUsuarios()
        {
            return mpUsuario.ListarUsuarios();
        }

        public LoginResult Login(UsuarioBE us)
        {
            LoginResult AuthOK;
            us.pass = Encriptador.EncriptarIrrev(us.pass);
            if (SessionManager.GetInstance.logged == false)
            {
                AuthOK = mpUsuario.Login(us);
                if (AuthOK == LoginResult.LoginOK)
                {
                    SessionManager.GetInstance.Login(us);
                    bitacora.RegistrarBitacora(us.user, TipoAccion.Login);
                    maxIntentos = 3;
                }
            }
            else
            {
                usuario = SessionManager.GetInstance.UsuarioActual();
                if ((usuario.user == us.user) && (usuario.pass == us.pass))
                {
                    AuthOK = LoginResult.SesionIniciada;
                    maxIntentos = 3;
                }
                else { AuthOK = LoginResult.ExisteSesion; }
            }
            if ((AuthOK != LoginResult.LoginOK) || (AuthOK != LoginResult.ExisteSesion)) { maxIntentos--; }
            if (maxIntentos == 0)
            {
                usuario = new UsuarioBE();
                usuario = mpUsuario.ExtraerUsuario(us.user);
                if (!string.IsNullOrEmpty(usuario.user))
                {
                    usuario.bloq = true;
                    usuario.pass = " ";
                    usuario.dvh = VerificadorIntegridad.CalcularDVH(usuario);
                    mpUsuario.ActualizarUsuario(usuario);
                }
                bitacora.RegistrarBitacora(us.user, TipoAccion.BloqueoUsuario);
                integridad.ActualizarDVV();
                AuthOK = LoginResult.FinIntentos;
            }
            bitacora.RegistrarBitacora(us.user, TipoAccion.LoginFail);
            return AuthOK;
        }

        public int VerifUsuario(UsuarioBE us, int tipo)//tipo 0 = Verificar Actual -- tipo == 1 Verificar Nueva
        {
            string encPass = Encriptador.EncriptarIrrev(us.pass);
            if (SessionManager.GetInstance.UsuarioActual().pass == encPass)
            {
                maxIntentos = 3;
                return 1;//Usuario Verificado OK -- Nueva = Anterior
            }
            else
            {
                if (tipo == 1)//Verificar Nueva
                {
                    UsuarioBE aux = SessionManager.GetInstance.UsuarioActual();
                    aux.pass = encPass;
                    aux.dvh = VerificadorIntegridad.CalcularDVH(aux);
                    mpUsuario.CambiarPass(aux);
                    maxIntentos = 3;
                    bitacora.RegistrarBitacora(aux.user, TipoAccion.CambioClave);
                    return 2;//Cambio de pass ok
                }
                maxIntentos--;
                if (maxIntentos == 0)
                {
                    if (tipo == 0)//Verificar Actual
                    {
                        UsuarioBE aux = SessionManager.GetInstance.UsuarioActual();
                        aux.bloq = true;
                        aux.dvh = VerificadorIntegridad.CalcularDVH(aux);
                        mpUsuario.ActualizarBloqueo(aux);
                        bitacora.RegistrarBitacora(aux.user, TipoAccion.BloqueoUsuario);
                        return 2; //Falla Verificacion, Bloquea usuario
                    }
                    else
                    {
                        maxIntentos = 3;
                        return 3;
                    }
                }
                bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.LoginFail);
                return 0; //Reintentar
            }
        }

        public void Logout()
        {
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.Logout);
            integridad.ActualizarDVV();
            SessionManager.GetInstance.Logout();
        }

        public void DesbloquearUS(UsuarioBE us)
        {
            usuario = us;
            us.pass = Encriptador.EncriptarIrrev(GenerarPass(us.ape, us.dni.ToString()));
            us.bloq = false;
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.ActualizarBloqueo(us);
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.DesbloqueoUsuario);
        }

        public void CrearUsuario(UsuarioBE us)
        {
            usuario = us;
            us.pass = Encriptador.EncriptarIrrev(us.pass);
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.CrearUsuario(us);
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.AltaUsuario);
        }

        public void EliminarUs(UsuarioBE us)
        {
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.EliminarUsuario(us);
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.BajaUsuario);
        }

        public void ActualizarUsuario(UsuarioBE us)
        {
            us.dvh = VerificadorIntegridad.CalcularDVH(us);
            mpUsuario.ActualizarUsuario(us);
            bitacora.RegistrarBitacora(SessionManager.GetInstance.UsuarioActual().user, TipoAccion.ModificacionUsuario);
        }

        public string GenerarPass(string ape, string dni)
        {
            string pass = ape.Substring(0, 3) + dni.Substring(0, 3);
            return pass;
        }


    }
}
