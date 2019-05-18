using System;

namespace Reheader
{
    public class HeaderInfo
    {
        public string CRC { get; set; }

        public GameConsole System { get; set; }

        public string Board { get; set; }

        public string PCB { get; set; }

        public string Chip { get; set; }
        
        public int Mapper { get; set; }

        public int PrgRomSizeKb { get; set; }

        public int ChrRomSizeKb { get; set; }

        public int ChrRamSizeKb { get; set; }

        public int WorkRamSizeKb { get; set; }

        public int SaveRamSizeKb { get; set; }

        public bool Battery { get; set; }

        public Mirroring Mirroring { get; set; }

        public string ControllerType { get; set; }

        public string BusConflicts { get; set; }

        public int SubMapper { get; set; }

        public string VsSystemType { get; set; }

        public string PpuModel { get; set; }

        public HeaderInfo(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
                return;

            var csvElements = csvLine.Split(',');

            if (csvElements.Length != 18)
                return;

            this.CRC = csvElements[0];
            this.System = Extensions.GameConsoleFromString(csvElements[1]);
            this.Board = csvElements[2];
            this.PCB = csvElements[3];
            this.Chip = csvElements[4];
            this.Mapper = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[5]) ? "-1" : csvElements[5]);
            this.PrgRomSizeKb = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[6]) ? "-1" : csvElements[6]);
            this.ChrRomSizeKb = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[7]) ? "-1" : csvElements[7]);
            this.ChrRamSizeKb = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[8]) ? "-1" : csvElements[8]);
            this.WorkRamSizeKb = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[9]) ? "-1" : csvElements[9]);
            this.SaveRamSizeKb = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[10]) ? "-1" : csvElements[10]);
            this.Battery = (csvElements[11] == "1");
            this.Mirroring = Extensions.MirroringFromString(csvElements[12]);
            this.ControllerType = csvElements[13];
            this.BusConflicts = csvElements[14];
            this.SubMapper = Int32.Parse(string.IsNullOrWhiteSpace(csvElements[15]) ? "-1" : csvElements[15]);
            this.VsSystemType = csvElements[16];
            this.PpuModel = csvElements[17];
        }

        /// <summary>
        /// Generate an iNES 1.0 or 2.0 header for the current Header information
        /// </summary>
        /// <param name="versionTwo">True if iNES 2.0 header should be created, false for iNES 1.0</param>
        /// <returns>Byte array representing the header, null on error</returns>
        public byte[] GenerateHeader(bool versionTwo)
        {
            if (this.CRC == null)
                return null;

            byte[] header = new byte[16];

            // NES signature
            header[0] = 0x4e;
            header[1] = 0x45;
            header[2] = 0x53;
            header[3] = 0x1a;

            // PRG ROM in 16KB units
            header[4] = (byte)(this.PrgRomSizeKb / 16);

            // CHR ROM in 8KB units
            header[5] = (byte)(this.ChrRamSizeKb > 0 ? 0 : this.ChrRomSizeKb / 8);

            // Mirroring, Battery, Trainer, Mapper
            header[6] = 0;

            // Mirroring
            if (this.Mirroring == Mirroring.Horizontal)
                header[6] &= 0x00;
            else if (this.Mirroring == Mirroring.Vertical)
                header[6] &= 0x01;
            else if (this.Mirroring == Mirroring.FourScreen)
                header[6] &= 0x08;

            // Battery
            if (this.Battery)
                header[6] &= 0x02;

            // Trainer
            //if (this.Trainer)
            //    header[6] &= 0x04;

            // Lower nibble of mapper
            header[6] &= (byte)(((byte)(this.Mapper & 0x0F)) << 4);

            // VS/Playchoice, NES 2.0, Mapper
            header[7] = 0;

            // VS Unisystem and PlayChoice-10
            if (this.System == GameConsole.VsSystem)
                header[7] &= 0x01;
            else if (this.System == GameConsole.Playchoice)
                header[7] &= 0x02;

            // NES 2.0 Format
            if (versionTwo)
                header[7] &= 0x08;

            // Upper nibble of mapper
            header[7] &= (byte)(this.Mapper & 0xF0);

            // iNES 1.0 Format
            if (!versionTwo)
            {
                // PRG-RAM size
                header[8] = (byte)(this.WorkRamSizeKb / 8);

                // TV System
                header[9] = 0;
                if (this.System == GameConsole.Famicom || this.System == GameConsole.NesNtsc)
                    header[9] &= 0x00;
                else if (this.System == GameConsole.Dendy || this.System == GameConsole.NesPal)
                    header[9] &= 0x01;

                // TV system, PRG-RAM presence, bus conflicts
                header[10] = 0;

                // TV System... again
                if (this.System == GameConsole.Famicom || this.System == GameConsole.NesNtsc)
                    header[9] &= 0x00;
                else if (this.System == GameConsole.Dendy || this.System == GameConsole.NesPal)
                    header[9] &= 0x02;

                // PRG-RAM presence
                if (this.WorkRamSizeKb > 0)
                    header[9] &= 0x10;

                // Bus conflicts
                if (!string.IsNullOrEmpty(this.BusConflicts) && this.BusConflicts != "N")
                    header[9] &= 0x20;

                // Padding
            }
            // iNES 2.0 Format
            else
            {
                // Mapper MSB, Submapper
                header[8] = 0;

                // Uppermost nibble of mapper
                header[8] &= (byte)((byte)(this.Mapper & 0xF00) >> 8);

                // Submapper
                header[8] &= (byte)(this.SubMapper << 4);

                // PRG-ROM/CHR-ROM size MSB
                //...

                // https://wiki.nesdev.com/w/index.php/NES_2.0
            }

            return header;
        }
    }
}
