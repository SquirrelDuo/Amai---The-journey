using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR || UNITY_STANDALONE
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
#endif

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script registers methods with SaveHelper that save games to disk
    /// instead of PlayerPrefs.
    /// </summary>
    public class SaveToDisk : MonoBehaviour
    {

#if UNITY_EDITOR || UNITY_STANDALONE

        [Tooltip("Encrypt saved game files.")]
        public bool encrypt = true;

        [Tooltip("If encrypting, use this password.")]
        public string encryptionPassword = "My Password";

        private class SavedGameInfo
        {
            public string summary;
            public string details;

            public SavedGameInfo(string summary, string details)
            {
                this.summary = summary;
                this.details = details;
            }
        }

        private List<SavedGameInfo> m_savedGameInfo = new List<SavedGameInfo>();
        private SaveHelper m_saveHelper = null;

        public void Start()
        {
            m_saveHelper = GetComponent<SaveHelper>();
            if (m_saveHelper != null)
            {
                m_saveHelper.SaveSlotHandler = SaveGameToDisk;
                m_saveHelper.LoadSlotHandler = LoadGameFromDisk;
                m_saveHelper.DeleteSlotHandler = DeleteGameFromDisk;
                LoadSavedGameInfoFromFile();
            }
        }

        public void SaveGameToDisk(int slotNum, string saveData)
        {
            if (DialogueDebug.LogWarnings) Debug.Log("Dialogue System Menus: Saving " + GetSaveGameFilename(slotNum));
            WriteStringToFile(GetSaveGameFilename(slotNum), encrypt ? Encrypt(saveData, encryptionPassword) : saveData);
            UpdateSavedGameInfoToFile(slotNum);
        }

        public string LoadGameFromDisk(int slotNum)
        {
            if (DialogueDebug.LogWarnings) Debug.Log("Dialogue System Menus: Loading " + GetSaveGameFilename(slotNum));
            var saveData = ReadStringFromFile(GetSaveGameFilename(slotNum));
            if (encrypt)
            {
                string plainText;
                return TryDecrypt(saveData, encryptionPassword, out plainText) ? plainText : string.Empty;
            }
            else
            {
                return saveData;
            }
        }

        public void DeleteGameFromDisk(int slotNum)
        {
            try
            {
                var filename = GetSaveGameFilename(slotNum);
                if (File.Exists(filename)) File.Delete(filename);
            }
            catch (System.Exception)
            {
            }
            UpdateSavedGameInfoToFile(slotNum);
        }

        public string GetSaveGameFilename(int slotNum)
        {
            return Application.persistentDataPath + "/save_" + slotNum + ".dat";
        }

        public string GetSavedGameInfoFilename()
        {
            return Application.persistentDataPath + "/saveinfo.dat";
        }

        public static void WriteStringToFile(string filename, string data)
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    streamWriter.WriteLine(data);
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Can't create file: " + filename);
            }
        }

        public static string ReadStringFromFile(string filename)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Error reading file: " + filename);
                return string.Empty;
            }
        }

        // From: https://developingsoftware.com/how-to-securely-store-data-in-unity-player-preferences

        const int Iterations = 1000;

        public string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(password)) return string.Empty;

            // create instance of the DES crypto provider
            var des = new DESCryptoServiceProvider();

            // generate a random IV will be used a salt value for generating key
            des.GenerateIV();

            // use derive bytes to generate a key from the password and IV
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, des.IV, Iterations);

            // generate a key from the password provided
            byte[] key = rfc2898DeriveBytes.GetBytes(8);

            // encrypt the plainText
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, des.CreateEncryptor(key, des.IV), CryptoStreamMode.Write))
            {
                // write the salt first not encrypted
                memoryStream.Write(des.IV, 0, des.IV.Length);

                // convert the plain text string into a byte array
                byte[] bytes = Encoding.UTF8.GetBytes(plainText);

                // write the bytes into the crypto stream so that they are encrypted bytes
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public bool TryDecrypt(string cipherText, string password, out string plainText)
        {
            // its pointless trying to decrypt if the cipher text
            // or password has not been supplied
            if (string.IsNullOrEmpty(cipherText) ||
                string.IsNullOrEmpty(password))
            {
                plainText = string.Empty;
                return false;
            }

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (var memoryStream = new MemoryStream(cipherBytes))
                {
                    // create instance of the DES crypto provider
                    var des = new DESCryptoServiceProvider();

                    // get the IV
                    byte[] iv = new byte[8];
                    memoryStream.Read(iv, 0, iv.Length);

                    // use derive bytes to generate key from password and IV
                    var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, iv, Iterations);

                    byte[] key = rfc2898DeriveBytes.GetBytes(8);

                    using (var cryptoStream = new CryptoStream(memoryStream, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        plainText = streamReader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Dialogue System Menus: Can't decrypt data: + " + ex.Message);
                plainText = string.Empty;
                return false;
            }
        }

        public void LoadSavedGameInfoFromFile()
        {
            var filename = GetSavedGameInfoFilename();
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) return;
            Debug.Log("Dialogue System Menus: Loading " + filename);
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    m_savedGameInfo.Clear();
                    int slotNum = 0;
                    int safeguard = 0;
                    while (!streamReader.EndOfStream && safeguard < 999)
                    {
                        var summary = streamReader.ReadLine().Replace("<cr>", "\n");
                        var details = streamReader.ReadLine().Replace("<cr>", "\n");
                        m_savedGameInfo.Add(new SavedGameInfo(summary, details));
                        if (string.IsNullOrEmpty(details))
                        {
                            PlayerPrefs.DeleteKey(m_saveHelper.GetSlotSummaryKey(slotNum));
                            PlayerPrefs.DeleteKey(m_saveHelper.GetSlotDetailsKey(slotNum));
                            PlayerPrefs.DeleteKey(m_saveHelper.GetSlotDataKey(slotNum));
                        }
                        else
                        {
                            PlayerPrefs.SetString(m_saveHelper.GetSlotSummaryKey(slotNum), summary);
                            PlayerPrefs.SetString(m_saveHelper.GetSlotDetailsKey(slotNum), details);
                            PlayerPrefs.SetString(m_saveHelper.GetSlotDataKey(slotNum), "nil");
                        }
                        slotNum++;
                        safeguard++;
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Error reading file: " + filename);
            }
        }

        public void UpdateSavedGameInfoToFile(int slotNum)
        {
            for (int i = m_savedGameInfo.Count; i <= slotNum; i++)
            {
                m_savedGameInfo.Add(new SavedGameInfo(PlayerPrefs.GetString(m_saveHelper.GetSlotSummaryKey(i)), PlayerPrefs.GetString(m_saveHelper.GetSlotDetailsKey(i))));
            }
            m_savedGameInfo[slotNum].summary = m_saveHelper.GetCurrentSummary(slotNum);
            m_savedGameInfo[slotNum].details = m_saveHelper.IsGameSavedInSlot(slotNum) ? m_saveHelper.GetCurrentDetails(slotNum) : string.Empty;
            var filename = GetSavedGameInfoFilename();
            Debug.Log("Dialogue System Menus: Updating " + filename);
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(filename))
                {
                    for (int i = 0; i < m_savedGameInfo.Count; i++)
                    {
                        streamWriter.WriteLine(m_savedGameInfo[i].summary.Replace("\n", "<cr>"));
                        streamWriter.WriteLine(m_savedGameInfo[i].details.Replace("\n", "<cr>"));
                    }
                }
            }
            catch (System.Exception)
            {
                Debug.LogError("Can't create file: " + filename);
            }
        }

#else
        void Start()
        {
            Debug.LogError("SaveToDisk is currently only supported in Standalone (desktop) builds.");
        }
#endif

    }

}