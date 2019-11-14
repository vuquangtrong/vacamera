using IMAPI2.Interop;
using IMAPI2.MediaItem;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace VACamera
{
    public partial class FormDvdWriter : Form
    {
        IMAPI_BURN_VERIFICATION_LEVEL _verificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;
        bool _isBurning1 = false;
        bool _isBurning2 = false;

        BurnData _burnData1 = new BurnData();
        BurnData _burnData2 = new BurnData();

        bool _isFinish1 = false;
        bool _isFinish2 = false;

        bool _closeMedia = true;
        bool _ejectMedia = true;

        string _sessionName = "";
        string _filePath = "";

        public FormDvdWriter(string sessionName, string filePath)
        {
            _sessionName = sessionName;
            _filePath = filePath;

            InitializeComponent();
        }

        private void FormDvdWriter_Load(object sender, EventArgs e)
        {
            txtFilename.Text = Path.GetFileName(_filePath);

            // Determine the current recording devices
            MsftDiscMaster2 discMaster = null;
            try
            {
                discMaster = new MsftDiscMaster2();

                if (!discMaster.IsSupportedEnvironment)
                {
                    MessageBox.Show("Không tìm thấy ổ đĩa phù hợp");

                    DialogResult = DialogResult.Cancel;
                    Hide();
                    Close();
                }

                foreach (string uniqueRecorderId in discMaster)
                {
                    var discRecorder2 = new MsftDiscRecorder2();
                    discRecorder2.InitializeDiscRecorder(uniqueRecorderId);

                    listDrive1.Items.Add(discRecorder2);
                    listDrive2.Items.Add(discRecorder2);
                }

                if (listDrive1.Items.Count <= 0)
                {
                    listDrive1.Enabled = false;
                    listDrive1.SelectedIndex = -1;

                    listDrive2.Enabled = false;
                    listDrive2.SelectedIndex = -1;
                }
                else if (listDrive1.Items.Count == 1)
                {
                    listDrive1.SelectedIndex = 0;

                    listDrive2.Enabled = false;
                    listDrive2.SelectedIndex = 0;
                }
                else
                {
                    listDrive1.SelectedIndex = 0;

                    listDrive2.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thiết bị không hỗ trợ thư viện IMAPI2");
                Log.WriteLine(ex.ToString());
                return;
            }
            finally
            {
                if (discMaster != null)
                {
                    Marshal.ReleaseComObject(discMaster);
                }
            }

            // Create the volume label based on the current date if needed
            if (_sessionName.Equals(""))
            {
                _sessionName = DateTime.Now.ToString("yyyyMMdd_HHmm");
            }
        }

        private void FormDvdWriter_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                foreach (MsftDiscRecorder2 discRecorder2 in listDrive1.Items)
                {
                    if (discRecorder2 != null)
                    {
                        try
                        {
                            Marshal.ReleaseComObject(discRecorder2);
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }

        private void listDrive_Format(object sender, ListControlConvertEventArgs e)
        {
            IDiscRecorder2 discRecorder2 = (IDiscRecorder2)e.ListItem;
            string devicePaths = string.Empty;
            string volumePath = (string)discRecorder2.VolumePathNames.GetValue(0);
            foreach (string volPath in discRecorder2.VolumePathNames)
            {
                if (!string.IsNullOrEmpty(devicePaths))
                {
                    devicePaths += ",";
                }
                devicePaths += volumePath;
            }

            e.Value = string.Format("{0} [{1}]", devicePaths, discRecorder2.ProductId);
        }

        private void btnWrite1_Click(object sender, EventArgs e)
        {
            if (listDrive1.SelectedIndex == -1)
            {
                return;
            }

            if (_isBurning1)
            {
                btnWrite1.Enabled = true;
                backgroundBurnWorker1.CancelAsync();
            }
            else
            {
                btnWrite1.Enabled = false;
                _isBurning1 = true;
                txtStatus1.Text = "Đang ghi...";

                var discRecorder = (IDiscRecorder2)listDrive1.Items[listDrive1.SelectedIndex];
                _burnData1.uniqueRecorderId = discRecorder.ActiveDiscRecorder;

                backgroundBurnWorker1.RunWorkerAsync(_burnData1);
            }

        }

        private void btnWrite2_Click(object sender, EventArgs e)
        {
            if (listDrive2.SelectedIndex == -1)
            {
                return;
            }

            if (_isBurning2)
            {
                btnWrite2.Enabled = false;
                backgroundBurnWorker2.CancelAsync();
            }
            else
            {
                _isBurning2 = true;
                txtStatus2.Text = "Đang ghi...";

                var discRecorder = (IDiscRecorder2)listDrive2.Items[listDrive2.SelectedIndex];
                _burnData2.uniqueRecorderId = discRecorder.ActiveDiscRecorder;

                backgroundBurnWorker2.RunWorkerAsync(_burnData1);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_isBurning1 || _isBurning2)
            {
                if (MessageBox.Show("Hủy ghi đĩa?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        backgroundBurnWorker1.CancelAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }

                    try
                    {
                        backgroundBurnWorker2.CancelAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                    }

                    Close();
                }
            }
            else
            {
                doAfterBurnWork();
            }
        }

        private void btnWriteAll_Click(object sender, EventArgs e)
        {
            if (btnWrite1.Enabled)
            {
                btnWrite1_Click(new object(), new EventArgs());
            }

            if (btnWrite2.Enabled)
            {
                btnWrite2_Click(new object(), new EventArgs());
            }
        }

        private void backgroundBurnWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Data discFormatData = null;
            try
            {
                // Create and initialize the IDiscRecorder2 object
                discRecorder = new MsftDiscRecorder2();
                var burnData = (BurnData)e.Argument;
                try
                {
                    Log.WriteLine("uniqueRecorderId = " + burnData.uniqueRecorderId);
                    discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);
                }
                catch (Exception ex)
                {
                    e.Result = -1;
                    Log.WriteLine(ex.ToString());
                    return;
                }

                // Create and initialize the IDiscFormat2Data
                discFormatData = new MsftDiscFormat2Data
                {
                    Recorder = discRecorder,
                    ClientName = "VACamera",
                    ForceMediaToBeClosed = _closeMedia
                };

                // Set the verification level
                var burnVerification = (IBurnVerification)discFormatData;
                burnVerification.BurnVerificationLevel = _verificationLevel;

                // Check if media is blank, (for RW media)
                object[] multisessionInterfaces = null;
                if (!discFormatData.MediaHeuristicallyBlank)
                {
                    multisessionInterfaces = discFormatData.MultisessionInterfaces;
                }

                // Create the file system
                IStream fileSystem;
                if (!createMediaFileSystem(discRecorder, multisessionInterfaces, out fileSystem))
                {
                    e.Result = -1;
                    Log.WriteLine("Cannot create filesystem on disk!");
                    return;
                }

                // add the Update event handler
                discFormatData.Update += discFormatData2_Update;

                // Write the data here
                try
                {
                    discFormatData.Write(fileSystem);
                    e.Result = 0;
                }
                catch (Exception ex)
                {
                    e.Result = -1;
                    Log.WriteLine(ex.ToString());
                }
                finally
                {
                    if (fileSystem != null)
                    {
                        Marshal.FinalReleaseComObject(fileSystem);
                    }
                }

                // remove the Update event handler
                discFormatData.Update -= discFormatData2_Update;

                if (_ejectMedia)
                {
                    discRecorder.EjectMedia();
                }
            }
            catch (Exception ex)
            {
                e.Result = -1;
                Log.WriteLine(ex.ToString());
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        private void backgroundBurnWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Data discFormatData = null;
            try
            {
                // Create and initialize the IDiscRecorder2 object
                discRecorder = new MsftDiscRecorder2();
                var burnData = (BurnData)e.Argument;
                try
                {
                    Log.WriteLine("uniqueRecorderId = " + burnData.uniqueRecorderId);
                    discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);
                }
                catch (Exception ex)
                {
                    e.Result = -1;
                    Log.WriteLine(ex.ToString());
                    return;
                }

                // Create and initialize the IDiscFormat2Data
                discFormatData = new MsftDiscFormat2Data
                {
                    Recorder = discRecorder,
                    ClientName = "VACamera",
                    ForceMediaToBeClosed = _closeMedia
                };

                // Set the verification level
                var burnVerification = (IBurnVerification)discFormatData;
                burnVerification.BurnVerificationLevel = _verificationLevel;

                // Check if media is blank, (for RW media)
                object[] multisessionInterfaces = null;
                if (!discFormatData.MediaHeuristicallyBlank)
                {
                    multisessionInterfaces = discFormatData.MultisessionInterfaces;
                }

                // Create the file system
                IStream fileSystem;
                if (!createMediaFileSystem(discRecorder, multisessionInterfaces, out fileSystem))
                {
                    e.Result = -1;
                    Log.WriteLine("Cannot create filesystem on disk!");
                    return;
                }

                // add the Update event handler
                discFormatData.Update += discFormatData2_Update;

                // Write the data here
                try
                {
                    discFormatData.Write(fileSystem);
                    e.Result = 0;
                }
                catch (Exception ex)
                {
                    e.Result = -1;
                    Log.WriteLine(ex.ToString());
                }
                finally
                {
                    if (fileSystem != null)
                    {
                        Marshal.FinalReleaseComObject(fileSystem);
                    }
                }

                // remove the Update event handler
                discFormatData.Update -= discFormatData2_Update;

                if (_ejectMedia)
                {
                    discRecorder.EjectMedia();
                }
            }
            catch (Exception ex)
            {
                e.Result = -1;
                Log.WriteLine(ex.ToString());
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        private void backgroundBurnWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var burnData = (BurnData)e.UserState;
            if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                txtStatus1.Text = burnData.statusMessage;
            }
            else if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.currentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        txtStatus1.Text = "Định dạng đĩa...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.lastWrittenLba - burnData.startLba;

                        if (writtenSectors > 0 && burnData.sectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.sectorCount);
                            txtStatus1.Text = string.Format("Tiến trình: {0}%", percent);
                            progressBar1.Value = percent;
                        }
                        else
                        {
                            txtStatus1.Text = "Tiến trình 0%";
                            progressBar1.Value = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        txtStatus1.Text = "Đang hoàn tất...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        txtStatus1.Text = "Đã hoàn thành!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        txtStatus1.Text = "Đang kiểm tra";
                        break;
                }
            }
        }

        private void backgroundBurnWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var burnData = (BurnData)e.UserState;
            if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                txtStatus2.Text = burnData.statusMessage;
            }
            else if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.currentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        txtStatus2.Text = "Định dạng đĩa...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.lastWrittenLba - burnData.startLba;

                        if (writtenSectors > 0 && burnData.sectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.sectorCount);
                            txtStatus2.Text = string.Format("Tiến trình: {0}%", percent);
                            progressBar2.Value = percent;
                        }
                        else
                        {
                            txtStatus2.Text = "Tiến trình 0%";
                            progressBar2.Value = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        txtStatus2.Text = "Đang hoàn tất...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        txtStatus2.Text = "Đã hoàn thành!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        txtStatus2.Text = "Đang kiểm tra";
                        break;
                }
            }
        }

        private void backgroundBurnWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtStatus1.Text = (int)e.Result == 0 ? "Đã ghi xong!" : "Có lỗi trong quá trình ghi đĩa!";
            progressBar1.Value = 0;

            _isBurning1 = false;
            _isFinish1 = true;
            btnWrite1.Enabled = true;

            doAfterBurnWork();
        }

        private void backgroundBurnWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtStatus2.Text = (int)e.Result == 0 ? "Đã ghi xong!" : "Có lỗi trong quá trình ghi đĩa!";
            progressBar2.Value = 0;

            _isBurning2 = false;
            _isFinish2 = true;
            btnWrite2.Enabled = true;

            doAfterBurnWork();
        }

        void discFormatData1_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (backgroundBurnWorker1.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;
                format2Data.CancelWrite();
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            _burnData1.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData1.elapsedTime = eventArgs.ElapsedTime;
            _burnData1.remainingTime = eventArgs.RemainingTime;
            _burnData1.totalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData1.currentAction = eventArgs.CurrentAction;
            _burnData1.startLba = eventArgs.StartLba;
            _burnData1.sectorCount = eventArgs.SectorCount;
            _burnData1.lastReadLba = eventArgs.LastReadLba;
            _burnData1.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData1.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData1.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData1.freeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            backgroundBurnWorker1.ReportProgress(0, _burnData1);
        }

        void discFormatData2_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (backgroundBurnWorker2.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;
                format2Data.CancelWrite();
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            _burnData2.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData2.elapsedTime = eventArgs.ElapsedTime;
            _burnData2.remainingTime = eventArgs.RemainingTime;
            _burnData2.totalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData2.currentAction = eventArgs.CurrentAction;
            _burnData2.startLba = eventArgs.StartLba;
            _burnData2.sectorCount = eventArgs.SectorCount;
            _burnData2.lastReadLba = eventArgs.LastReadLba;
            _burnData2.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData2.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData2.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData2.freeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            backgroundBurnWorker2.ReportProgress(0, _burnData2);
        }

        private bool createMediaFileSystem(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, out IStream dataStream)
        {
            MsftFileSystemImage fileSystemImage = null;
            try
            {
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaults(discRecorder);
                fileSystemImage.FileSystemsToCreate = FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                fileSystemImage.VolumeName = _sessionName;

                // If multisessions, then import previous sessions
                if (multisessionInterfaces != null)
                {
                    try
                    {
                        fileSystemImage.MultisessionInterfaces = multisessionInterfaces;
                        fileSystemImage.ImportFileSystem();
                    }
                    catch (Exception ex)
                    {
                        // ignore multisession
                        Log.WriteLine(ex.ToString());
                    }
                }

                // Get the image root
                IFsiDirectoryItem rootItem = fileSystemImage.Root;

                // Add Files and Directories to File System Image
                var fileItem = new FileItem(_filePath);
                IMediaItem mediaItem = fileItem;
                mediaItem.AddToFileSystem(rootItem);

                // Make data stream
                try
                {
                    dataStream = fileSystemImage.CreateResultImage().ImageStream;
                }
                catch (Exception ex)
                {
                    dataStream = null;
                    MessageBox.Show("Ổ đĩa bị khóa hoặc có lỗi trong quá trình định dạng đĩa");
                    Log.WriteLine(ex.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                dataStream = null;
                MessageBox.Show("Ổ đĩa bị khóa hoặc có lỗi trong quá trình định dạng đĩa");
                Log.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }

            return true;
        }

        private static string GetMediaTypeString(IMAPI_MEDIA_PHYSICAL_TYPE mediaType)
        {
            switch (mediaType)
            {
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_UNKNOWN:
                default:
                    return "Unknown Media Type";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDROM:
                    return "CD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDR:
                    return "CD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW:
                    return "CD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDROM:
                    return "DVD ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM:
                    return "DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR:
                    return "DVD+R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW:
                    return "DVD+RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR_DUALLAYER:
                    return "DVD+R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR:
                    return "DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW:
                    return "DVD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR_DUALLAYER:
                    return "DVD-R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DISK:
                    return "random-access writes";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW_DUALLAYER:
                    return "DVD+RW DL";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDROM:
                    return "HD DVD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDR:
                    return "HD DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDRAM:
                    return "HD DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDROM:
                    return "Blu-ray DVD (BD-ROM)";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDR:
                    return "Blu-ray media";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDRE:
                    return "Blu-ray Rewritable media";
            }
        }

        private bool detectMedia1()
        {
            if (listDrive1.SelectedIndex == -1)
            {
                txtStatus1.Text = "Không có ổ đĩa";
                return false;
            }

            var discRecorder = (IDiscRecorder2)listDrive1.Items[listDrive1.SelectedIndex];

            MsftFileSystemImage fileSystemImage = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                // Create and initialize the IDiscFormat2Data
                discFormatData = new MsftDiscFormat2Data();
                if (!discFormatData.IsCurrentMediaSupported(discRecorder))
                {
                    txtStatus1.Text = "Ổ đĩa không có chức năng ghi";
                    return false;
                }
                else
                {
                    // Get the media type in the recorder
                    discFormatData.Recorder = discRecorder;
                    IMAPI_MEDIA_PHYSICAL_TYPE mediaType = discFormatData.CurrentPhysicalMediaType;
                    txtStatus1.Text = GetMediaTypeString(mediaType);

                    // Create a file system and select the media type
                    fileSystemImage = new MsftFileSystemImage();
                    fileSystemImage.ChooseImageDefaultsForMediaType(mediaType);

                    // See if there are other recorded sessions on the disc
                    if (!discFormatData.MediaHeuristicallyBlank)
                    {
                        try
                        {
                            fileSystemImage.MultisessionInterfaces = discFormatData.MultisessionInterfaces;
                            fileSystemImage.ImportFileSystem();
                        }
                        catch (Exception ex)
                        {
                            txtStatus1.Text = GetMediaTypeString(mediaType) + " - " + "Đĩa đã bị khóa chức năng ghi.";
                            Log.WriteLine(ex.ToString());
                            return false;
                        }
                    }

                    Int64 freeMediaBlocks = fileSystemImage.FreeMediaBlocks;
                    long _totalDiscSize = 2048 * freeMediaBlocks;

                    txtStatus1.Text = GetMediaTypeString(mediaType) + " - " + "Dung lượng trống: " + (_totalDiscSize < 1000000000 ?
                        string.Format("{0}MB", _totalDiscSize / 1000000) :
                        string.Format("{0:F2}GB", (float)_totalDiscSize / 1000000000.0));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        private bool detectMedia2()
        {
            if (listDrive2.SelectedIndex == -1)
            {
                txtStatus2.Text = "Không có ổ đĩa";
                return false;
            }

            var discRecorder = (IDiscRecorder2)listDrive2.Items[listDrive2.SelectedIndex];

            MsftFileSystemImage fileSystemImage = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                // Create and initialize the IDiscFormat2Data
                discFormatData = new MsftDiscFormat2Data();
                if (!discFormatData.IsCurrentMediaSupported(discRecorder))
                {
                    txtStatus2.Text = "Ổ đĩa không có chức năng ghi";
                    return false;
                }
                else
                {
                    // Get the media type in the recorder
                    discFormatData.Recorder = discRecorder;
                    IMAPI_MEDIA_PHYSICAL_TYPE mediaType = discFormatData.CurrentPhysicalMediaType;
                    txtStatus2.Text = GetMediaTypeString(mediaType);

                    // Create a file system and select the media type
                    fileSystemImage = new MsftFileSystemImage();
                    fileSystemImage.ChooseImageDefaultsForMediaType(mediaType);

                    // See if there are other recorded sessions on the disc
                    if (!discFormatData.MediaHeuristicallyBlank)
                    {
                        try
                        {
                            fileSystemImage.MultisessionInterfaces = discFormatData.MultisessionInterfaces;
                            fileSystemImage.ImportFileSystem();
                        }
                        catch (Exception ex)
                        {
                            txtStatus2.Text = GetMediaTypeString(mediaType) + " - " + "Đĩa đã bị khóa chức năng ghi.";
                            Log.WriteLine(ex.ToString());
                            return false;
                        }
                    }

                    Int64 freeMediaBlocks = fileSystemImage.FreeMediaBlocks;
                    long _totalDiscSize = 2048 * freeMediaBlocks;

                    txtStatus2.Text = GetMediaTypeString(mediaType) + " - " + "Dung lượng trống: " + (_totalDiscSize < 1000000000 ?
                        string.Format("{0}MB", _totalDiscSize / 1000000) :
                        string.Format("{0:F2}GB", (float)_totalDiscSize / 1000000000.0));
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                return false;
            }

            return true;

        }

        private void listDrive1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnWrite1.Enabled = detectMedia1();
            if (listDrive2.SelectedIndex == listDrive1.SelectedIndex)
            {
                txtStatus2.Text = "Hãy chọn ổ đĩa khác";
                btnWrite2.Enabled = false;
            }
        }

        private void listDrive2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listDrive2.SelectedIndex == listDrive1.SelectedIndex)
            {
                txtStatus2.Text = "Hãy chọn ổ đĩa khác";
                btnWrite2.Enabled = false;
            }
            else
            {
                btnWrite2.Enabled = detectMedia2();
            }
        }

        private void doAfterBurnWork()
        {
            if (!(_isBurning1 || _isBurning2))
            {
                // clean file
                try
                {
                    File.Delete(_filePath);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.ToString());
                }

                // show new session
                if (MessageBox.Show("Bạn có muốn tạo phiên làm việc mới không?", "Tiếp tục", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // insert new disk
                    while (true)
                    {
                        if (MessageBox.Show("Hãy đưa đĩa mới vào ổ đĩa và nhấn OK để tiếp tục", "Yêu cầu", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            if (CountNewDisk() > 0)
                            {
                                DialogResult = DialogResult.Yes;
                                Close();
                            }
                        } // cancel to force close
                        else
                        {
                            DialogResult = DialogResult.No;
                            Close();
                        }
                    }
                }
                else
                {
                    DialogResult = DialogResult.No;
                    Close();
                }
            }
        }

        private int CountNewDisk()
        {
            int count = 0;
            foreach (MsftDiscRecorder2 discRecorder in listDrive2.Items)
            {
                if (IsMediaWritable(discRecorder))
                {
                    count++;
                }
            }
            return count;
        }

        private bool IsMediaWritable(MsftDiscRecorder2 discRecorder)
        {
            MsftFileSystemImage fileSystemImage = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                // Create and initialize the IDiscFormat2Data
                discFormatData = new MsftDiscFormat2Data();
                if (!discFormatData.IsCurrentMediaSupported(discRecorder))
                {
                    return false;
                }
                else
                {
                    // Get the media type in the recorder
                    discFormatData.Recorder = discRecorder;
                    IMAPI_MEDIA_PHYSICAL_TYPE mediaType = discFormatData.CurrentPhysicalMediaType;

                    // Create a file system and select the media type
                    fileSystemImage = new MsftFileSystemImage();
                    fileSystemImage.ChooseImageDefaultsForMediaType(mediaType);

                    // See if there are other recorded sessions on the disc
                    if (!discFormatData.MediaHeuristicallyBlank)
                    {
                        try
                        {
                            fileSystemImage.MultisessionInterfaces = discFormatData.MultisessionInterfaces;
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine(ex.ToString());
                            return false;
                        }
                        fileSystemImage.ImportFileSystem();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }
}