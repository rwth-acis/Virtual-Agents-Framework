#!/bin/bash

# !!!When running the script, pass the project version as an argument!!!
version=$1
versionReplaceString="s/\${version}/$version/g"

# =============== README =====================

readmeMainPath='../README.md'
readmeDocPath='../Documentation/index.md'
readmePackagePath='../Assets/Virtual Agents Framework/README.md'

# Copy modified README to repository front page, documentation and package
cp README.md "$readmeMainPath"
cp README.md "$readmeDocPath"
cp README.md "$readmePackagePath"

# replace the version and paths
sed -i -e "$versionReplaceString" -e "s/\${docPath}/https:\/\/rwth-acis.github.io\/Virtual-Agents-Framework\/$version\//g" -e "s/\${docImgPath}/Documentation\//g" -e "s/\${docExtension}/html/g" "$readmeMainPath"
sed -i -e "$versionReplaceString" -e "s/\${docPath}//g" -e "s/\${docImgPath}//g" -e "s/\${docExtension}/md/g" "$readmeDocPath"
sed -i -e "$versionReplaceString" -e "s/\${docPath}/https:\/\/rwth-acis.github.io\/Virtual-Agents-Framework\/$version\//g" -e "s/\${docImgPath}/https:\/\/rwth-acis.github.io\/Virtual-Agents-Framework\/$version\//g" -e "s/\${docExtension}/html/" "$readmePackagePath"

# =============== Documentation =====================

docPackagePath='../Assets/Virtual Agents Framework/Documentation~/Virtual-Agents-Framework.md'

# Copy Documentation file to package
cp Documentation.md "$docPackagePath"

# replace the version
sed -i "$versionReplaceString" "$docPackagePath"

# =============== Package =====================

packageJsonPath='../Assets/Virtual Agents Framework/package.json'

# Copy package file to package
cp package.json "$packageJsonPath"

# replace the version
sed -i "$versionReplaceString" "$packageJsonPath"

# =============== CHANGELOG =====================

# Copy changelog to repository front page and package
cp CHANGELOG.md ../CHANGELOG.md
cp CHANGELOG.md ../Assets/'Virtual Agents Framework'/CHANGELOG.md

# =============== Project Settings =====================
sed -i "/bundleVersion:/s/.*/  bundleVersion: $version/" "../ProjectSettings/ProjectSettings.asset"