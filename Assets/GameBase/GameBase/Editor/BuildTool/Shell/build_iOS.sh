# #!/bin/bash
# From https://www.xuanyusong.com/archives/2734

#参数判断
if [ $# != 3 ];then
    echo "Params error!"
    echo "Need two params: 1.path of project 2.name of ipa file"
    exit
elif [ ! -d $1 ];then
    echo "The first param is not a dictionary."
    exit
fi

#工程路径
project_path=$1

#IPA名称
ipa_name=$2

source_app_name=$3

#build文件夹路径
build_path=${project_path}/build

#编译工程
cd $project_path

#清理#
xcodebuild clean

echo "Tip : xcodebuild begin"
xcodebuild || exit
echo "Tip : xcodebuild success"

echo "Tip : xcode app to ipa begin"
xcrun -sdk iphoneos PackageApplication -v ${build_path}/Release-iphoneos/${source_app_name}.app -o ${project_path}/../../ios/${ipa_name}.ipa
echo "Tip : xcode app to ipa success"
