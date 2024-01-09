using System;
using System.IO;
using UnityEngine;

namespace YooAsset.Editor
{
    [DisplayName("定位地址: 文件名")]
    public class AddressByFileName : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileNameWithoutExtension(data.AssetPath);
        }
    }

    [DisplayName("定位地址: 文件名(带后缀)")]
    public class AddressByFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileName(data.AssetPath);
        }
    }


    [DisplayName("定位地址: 文件路径")]
    public class AddressByFilePath : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath;
        }
    }

    [DisplayName("定位地址: 分组名+文件名")]
    public class AddressByGroupAndFileName : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileNameWithoutExtension(data.AssetPath);
            return $"{data.GroupName}/{fileName}";
        }
    }

    [DisplayName("定位地址: 分组名+文件名(带后缀)")]
    public class AddressByGroupAndFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileName(data.AssetPath);
            return $"{data.GroupName}/{fileName}";
        }
    }


    [DisplayName("定位地址: 文件夹名+文件名")]
    public class AddressByFolderAndFileName : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileNameWithoutExtension(data.AssetPath);
            FileInfo fileInfo = new FileInfo(data.AssetPath);
            return $"{fileInfo.Directory.Name}/{fileName}";
        }
    }

    [DisplayName("定位地址: 文件夹名+文件名(带后缀)")]
    public class AddressByFolderAndFileNameWithExt : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string fileName = Path.GetFileName(data.AssetPath);
            FileInfo fileInfo = new FileInfo(data.AssetPath);
            return $"{fileInfo.Directory.Name}/{fileName}";
        }
    }

    //TODO: 待重构规则
    [DisplayName("定位地址: 文件路径  剔除部分前缀(Assets_Arts_  Skin_) ")]
    public class AddressByFolderAndWithoutName : IAddressRule
    {
        const string ReplaceStr = "Assets/Arts/";

        const string ReplaceStr2 = "Skin/";

        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string all = data.AssetPath;

            string res = all.Replace(ReplaceStr, "");

            string res2 = res.Replace(ReplaceStr2, "");

            return res2;
        }
    }

    //TODO: 待重构规则
    [DisplayName("定位地址: UI + 文件名称 ")]
    public class AddressByUISpecial : IAddressRule
    {
        const string UIStr = "UI/";

        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string name = Path.GetFileName(data.AssetPath);

            string all = UIStr + name;

            return all;
        }
    }
    //TODO: 待重构规则
    [DisplayName("定位地址: 文件路径  剔除部分前缀(Assets_Arts_  Skin_) ")]
    public class AddressBySpecialUI : IAddressRule
    {
        const string ReplaceStr = "Assets/Arts/";

        const string ReplaceStr2 = "Skin/";

        const string ReplaceStr3 = "Skin/";

        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string all = data.AssetPath;

            string res = all.Replace(ReplaceStr, "");

            string res2 = res.Replace(ReplaceStr2, "");

            return res2;
        }
    }

    //TODO: 待重构规则
    [DisplayName("精灵的散图专用路径 剔除__Textures")]
    public class AddressBySpriteTexturesUI : IAddressRule
    {
        const string ReplaceStr = "Assets/Prefabs/UI/";
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {

            string path = data.AssetPath.Replace(ReplaceStr, "");

            //return path;
            // 获取最后一个文件夹的索引
            int lastFolderIndex = path.LastIndexOf('/');
            if (lastFolderIndex != -1)
            {
                // 文件名
                string pathLeft = path.Substring(0, lastFolderIndex - 1);
                int secondIndex = pathLeft.LastIndexOf("/");
                string leftPart = path.Substring(0, secondIndex);

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                string combinedPath = Path.Combine(leftPart, fileNameWithoutExtension);
                combinedPath = combinedPath.Replace("\\", "/");
                return combinedPath;
            }
            else
            {
                Debug.LogError("error path:" + path);
                return null;
            }

        }
    }

    //TODO: 待重构规则
    [DisplayName("Raw纹理原图图专用路径 __RawTex下的单张原图")]
    public class AddressByRawTexturesUI : IAddressRule
    {
        const string ReplaceStr = "Assets/Prefabs/UI/";
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            string path = data.AssetPath.Replace(ReplaceStr, "");
            //return path;
            // 获取最后一个文件夹的索引
            int lastFolderIndex = path.LastIndexOf('/');
            if (lastFolderIndex != -1)
            {
                // 文件名
                string pathLeft = path.Substring(0, lastFolderIndex - 1);
                int secondIndex = pathLeft.LastIndexOf("/");
                string leftPart = path.Substring(0, secondIndex);

                string fileNameWithoutExtension = Path.GetFileName(path);
                string combinedPath = Path.Combine(leftPart, fileNameWithoutExtension);
                combinedPath = combinedPath.Replace("\\", "/");
                return combinedPath;
            }
            else
            {
                Debug.LogError("error path:" + path);
                return null;
            }

        }
    }

}