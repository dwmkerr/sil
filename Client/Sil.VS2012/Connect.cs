using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Sil.VSCommon;

namespace Sil.VS2012
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        /// <summary>
        /// Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="connectMode">The connect mode.</param>
        /// <param name="addInInst">The add in inst.</param>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            silAddinInstance = new SilAddinInstance((DTE2) application, (AddIn) addInInst);

            switch (connectMode)
            {
                case ext_ConnectMode.ext_cm_UISetup:

                    //  Create commands in the UI Setup phase. 
                    //  This phase is called only once when the add-in is deployed.
                    silAddinInstance.CreateCommands();

                    break;

                case ext_ConnectMode.ext_cm_AfterStartup:

                    //  After startup is complete, initialise the addin.
                    silAddinInstance.CreateUserInterface();

                    break;
            }
        }

        /// <summary>
        /// Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.
        /// </summary>
        /// <param name="disconnectMode">The disconnect mode.</param>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            switch (disconnectMode)
            {
                case ext_DisconnectMode.ext_dm_HostShutdown:
                case ext_DisconnectMode.ext_dm_UserClosed:

                    silAddinInstance.DestroyUserInterface();

                    break;
            }
        }

        /// <summary>
        /// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>
        /// Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
        public void OnStartupComplete(ref Array custom)
        {
            if (silAddinInstance != null)
                silAddinInstance.CreateUserInterface();
        }

        /// <summary>
        /// Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>
        /// Queries the status.
        /// </summary>
        /// <param name="CmdName">Name of the CMD.</param>
        /// <param name="NeededText">The needed text.</param>
        /// <param name="StatusOption">The status option.</param>
        /// <param name="CommandText">The command text.</param>
        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (silAddinInstance != null)
                silAddinInstance.QueryCommandStatus(CmdName, NeededText, ref StatusOption, ref CommandText);
        }

        /// <summary>
        /// Execs the specified CMD name.
        /// </summary>
        /// <param name="CmdName">Name of the CMD.</param>
        /// <param name="ExecuteOption">The execute option.</param>
        /// <param name="VariantIn">The variant in.</param>
        /// <param name="VariantOut">The variant out.</param>
        /// <param name="Handled">if set to <c>true</c> [handled].</param>
        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            if (silAddinInstance != null)
                silAddinInstance.ExecuteCommand(CmdName, ExecuteOption, ref VariantIn, ref VariantOut, ref Handled);

        }

        private SilAddinInstance silAddinInstance;
    }
}