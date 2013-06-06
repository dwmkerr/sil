rem Build folder structure.
rmdir /s /q "DeploymentPackage"
mkdir "DeploymentPackage"
mkdir "DeploymentPackage\Sil.VS2010"
mkdir "DeploymentPackage\Sil.VS2010\en-US"
mkdir "DeploymentPackage\Sil.VS2012"
mkdir "DeploymentPackage\Sil.VS2012\en-US"
mkdir "DeploymentPackage\Sil"

rem Create Sil.2010
copy .\Client\Sil.VS2010\bin\release\Sil.VS2010.dll .\DeploymentPackage\Sil.VS2010
copy .\Client\Sil.VS2010\Sil.VS2010.addin .\DeploymentPackage\Sil.VS2010
cd .\Client\Sil.VS2010\
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\resgen.exe" Resources.resx
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\al.exe" /t:lib /embed:Resources.resources /culture:en-US /out:Sil.VS2010.Resources.dll
cd ..\..\
copy .\Client\Sil.VS2010\Sil.VS2010.Resources.dll DeploymentPackage\Sil.VS2010\en-US

rem Create Sil.2012
copy .\Client\Sil.VS2012\bin\release\Sil.VS2012.dll .\DeploymentPackage\Sil.VS2012
copy .\Client\Sil.VS2012\Sil.VS2012.addin .\DeploymentPackage\Sil.VS2012
cd .\Client\Sil.VS2012\
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\resgen.exe" Resources.resx
"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\al.exe" /t:lib /embed:Resources.resources /culture:en-US /out:Sil.VS2012.Resources.dll
cd ..\..\
copy .\Client\Sil.VS2012\Sil.VS2012.Resources.dll DeploymentPackage\Sil.VS2012\en-US

rem Create Standalone
copy .\Client\Sil\bin\Release\Sil.exe .\DeploymentPackage\Sil
copy .\Client\Sil\bin\Release\*.dll .\DeploymentPackage\Sil