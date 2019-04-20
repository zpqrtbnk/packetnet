/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Copyright 2012 Alan Rushforth <alan.rushforth@gmail.com>
 */

using System;
using System.Net.NetworkInformation;
using PacketDotNet.MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Ieee80211
{
    /// <summary>
    /// Block acknowledgment request frame.
    /// </summary>
    public sealed class BlockAcknowledgmentRequestFrame : MacFrame
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="byteArraySegment">
        /// A <see cref="ByteArraySegment" />
        /// </param>
        public BlockAcknowledgmentRequestFrame(ByteArraySegment byteArraySegment)
        {
            Header = new ByteArraySegment(byteArraySegment);

            FrameControl = new FrameControlField(FrameControlBytes);
            Duration = new DurationField(DurationBytes);
            ReceiverAddress = GetAddress(0);
            TransmitterAddress = GetAddress(1);
            BlockAcknowledgmentControl = new BlockAcknowledgmentControlField(BlockAckRequestControlBytes);
            BlockAckStartingSequenceControl = BlockAckStartingSequenceControlBytes;

            Header.Length = FrameSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockAcknowledgmentRequestFrame" /> class.
        /// </summary>
        /// <param name='transmitterAddress'>
        /// Transmitter address.
        /// </param>
        /// <param name='receiverAddress'>
        /// Receiver address.
        /// </param>
        public BlockAcknowledgmentRequestFrame
        (
            PhysicalAddress transmitterAddress,
            PhysicalAddress receiverAddress)
        {
            FrameControl = new FrameControlField();
            Duration = new DurationField();
            ReceiverAddress = receiverAddress;
            TransmitterAddress = transmitterAddress;
            BlockAcknowledgmentControl = new BlockAcknowledgmentControlField();

            FrameControl.SubType = FrameControlField.FrameSubTypes.ControlBlockAcknowledgmentRequest;
        }

        /// <summary>
        /// Block acknowledgment control field
        /// </summary>
        public BlockAcknowledgmentControlField BlockAcknowledgmentControl { get; set; }

        /// <summary>
        /// Gets or sets the sequence number of the first MSDU for which this
        /// block acknowledgement request frame is sent
        /// </summary>
        /// <value>
        /// The block ack starting sequence control field value
        /// </value>
        public UInt16 BlockAckStartingSequenceControl { get; set; }


        /// <summary>
        /// Length of the frame
        /// </summary>
        public override Int32 FrameSize => MacFields.FrameControlLength +
                                           MacFields.DurationIDLength +
                                           (MacFields.AddressLength * 2) +
                                           BlockAckRequestFields.BlockAckRequestControlLength +
                                           BlockAckRequestFields.BlockAckStartingSequenceControlLength;

        /// <summary>
        /// Receiver address
        /// </summary>
        public PhysicalAddress ReceiverAddress { get; set; }

        /// <summary>
        /// Transmitter address
        /// </summary>
        public PhysicalAddress TransmitterAddress { get; set; }

        /// <summary>
        /// Block acknowledgment control bytes are the first two bytes of the frame
        /// </summary>
        private UInt16 BlockAckRequestControlBytes
        {
            get
            {
                if (Header.Length >=
                    BlockAckRequestFields.BlockAckRequestControlPosition +
                    BlockAckRequestFields.BlockAckRequestControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + BlockAckRequestFields.BlockAckRequestControlPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BlockAckRequestFields.BlockAckRequestControlPosition);
        }

        /// <summary>
        /// Gets or sets the block ack starting sequence control.
        /// </summary>
        /// <value>
        /// The block ack starting sequence control.
        /// </value>
        private UInt16 BlockAckStartingSequenceControlBytes
        {
            get
            {
                if (Header.Length >=
                    BlockAckRequestFields.BlockAckStartingSequenceControlPosition +
                    BlockAckRequestFields.BlockAckStartingSequenceControlLength)
                {
                    return EndianBitConverter.Little.ToUInt16(Header.Bytes,
                                                              Header.Offset + BlockAckRequestFields.BlockAckStartingSequenceControlPosition);
                }

                return 0;
            }
            set => EndianBitConverter.Little.CopyBytes(value,
                                                       Header.Bytes,
                                                       Header.Offset + BlockAckRequestFields.BlockAckStartingSequenceControlPosition);
        }

        /// <summary>
        /// Writes the current packet properties to the backing ByteArraySegment.
        /// </summary>
        public override void UpdateCalculatedValues()
        {
            if (Header == null || Header.Length > Header.BytesLength - Header.Offset || Header.Length < FrameSize)
            {
                Header = new ByteArraySegment(new Byte[FrameSize]);
            }

            FrameControlBytes = FrameControl.Field;
            DurationBytes = Duration.Field;
            SetAddress(0, ReceiverAddress);
            SetAddress(1, TransmitterAddress);

            BlockAckRequestControlBytes = BlockAcknowledgmentControl.Field;
            BlockAckStartingSequenceControlBytes = BlockAckStartingSequenceControl;

            Header.Length = FrameSize;
        }

        /// <summary>
        /// Returns a string with a description of the addresses used in the packet.
        /// This is used as a compoent of the string returned by ToString().
        /// </summary>
        /// <returns>
        /// The address string.
        /// </returns>
        protected override String GetAddressString()
        {
            return $"RA {ReceiverAddress} TA {TransmitterAddress}";
        }
    }
}