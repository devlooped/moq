/*
Copyright (c) 2007-2008, The Cosmos Project
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of The Cosmos Project nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Indy.IL2CPU.IL {
	internal partial class ILReader {
		private readonly MethodBody mBody;
		private MemoryStream mStream;
		private MethodBase mMethod;
		private Module mModule;
		public ILReader(MethodBase aMethod) {
			mBody = aMethod.GetMethodBody();
			mMethod = aMethod;
			mModule = mMethod.Module;
			mStream = new MemoryStream(mBody.GetILAsByteArray());
		}

		private OpCodeEnum mOpCode;
		private byte[] mOperand;
		private bool mHasOperand;

		public uint Position {
			get;
			private set;
		}

		public uint NextPosition {
			get {
				return (uint)mStream.Position;
			}
		}

		public bool EndOfStream {
			get {
				return mStream.Position == mStream.Length;
			}
		}

		public bool HasOperand {
			get {
				return mHasOperand;
			}
		}

		public OpCodeEnum OpCode {
			get {
				return mOpCode;
			}
		}

		public byte[] Operand {
			get {
				return mOperand;
			}
		}

		private string mOperandValueStr;
		public string OperandValueStr {
			get {
				if (mOperandValueStr == null) {
					mOperandValueStr = mModule.ResolveString(OperandValueInt32);
				}
				return mOperandValueStr;
			}
		}

		private MethodBase mOperandValueMethod;
		public MethodBase OperandValueMethod {
			get {
				if (mOperandValueMethod == null) {
					Type[] xTypeGenArgs = null;
					Type[] xMethodGenArgs = null;
					if (mMethod.DeclaringType.IsGenericType) {
						xTypeGenArgs = mMethod.DeclaringType.GetGenericArguments();
					}
					if (mMethod.IsGenericMethod) {
						xMethodGenArgs = mMethod.GetGenericArguments();
					}
					mOperandValueMethod = mModule.ResolveMethod(OperandValueInt32, xTypeGenArgs, xMethodGenArgs);
				}
				return mOperandValueMethod;
			}
		}

		private uint? mOperandValueBranchPosition;
		private bool mIsShortcut;
		public uint OperandValueBranchPosition {
			get {
				if (mOperandValueBranchPosition == null) {
					//sbyte xTemp = (sbyte)mOperand;
					//if (xTemp == mOperand) {
					//    mOperandValueBranchPosition = NextPosition + xTemp;
					//} else {
					//						if (mStream.Length < (NextPosition + mOperand + 1)) {
					//							mOperandValueBranchPosition = (uint)mOperand;
					//						} else {
					//							mOperandValueBranchPosition = (uint)(NextPosition + mOperand);
					//						}
					if (!mIsShortcut) {
						mOperandValueBranchPosition = (uint?)(NextPosition + OperandValueInt32);
					} else {
						mOperandValueBranchPosition = (uint?)(NextPosition + (sbyte)OperandValueInt32);
					}
					//}
				}
				return mOperandValueBranchPosition.Value;
			}
		}

		private FieldInfo mOperandValueField;
		public FieldInfo OperandValueField {
			get {
                if (mOperandValueField == null) {
                    try
                    {
                        Type[] xTypeGenArgs = null;
                        Type[] xMethodGenArgs = null;
                        if (mMethod.DeclaringType.IsGenericType)
                        {
                            xTypeGenArgs = mMethod.DeclaringType.GetGenericArguments();
                        }
                        if (mMethod.IsGenericMethod)
                        {
                            xMethodGenArgs = mMethod.GetGenericArguments();
                        }
                        mOperandValueField = mModule.ResolveField(OperandValueInt32,
                                                                  xTypeGenArgs,
                                                                  xMethodGenArgs);
                    }
                    catch {
                    }
                }
			    return mOperandValueField;
			}
		}

		private Type mOperandValueType;
		public Type OperandValueType {
			get {
				if (mOperandValueType == null) {
                   try{
                        Type[] xTypeGenArgs = null;
                        Type[] xMethodGenArgs = null;
                        if (mMethod.DeclaringType.IsGenericType) {
                            xTypeGenArgs = mMethod.DeclaringType.GetGenericArguments();
                        }
                        if (mMethod.IsGenericMethod) {
                            xMethodGenArgs = mMethod.GetGenericArguments();
                        }
                        mOperandValueType = mModule.ResolveType(OperandValueInt32,
                                                                xTypeGenArgs,
                                                                xMethodGenArgs);
                    }catch {
                    }
				}
				return mOperandValueType;
			}
		}

		public uint[] OperandValueBranchLocations {
			get;
			private set;
		}

		private int? mOperandValueInt32;
		public int OperandValueInt32 {
            get
            {
                if (!mIsShortcut)
                {
                    if (mOperandValueInt32 == null)
                    {
                        byte[] xData = new byte[4];
                        Array.Copy(Operand, xData, Math.Min(4, Operand.Length));
                        mOperandValueInt32 = BitConverter.ToInt32(xData, 0);
                    }
                }
                else 
                { 
                    sbyte xShortValue = (sbyte)Operand[0]; 
                    mOperandValueInt32 = xShortValue; 
                }
                return mOperandValueInt32.Value;
            }
		}

		private Single? mOperandValueSingle;
		public Single OperandValueSingle {
			get {
				if (mOperandValueSingle == null) {
					mOperandValueSingle = BitConverter.ToSingle(Operand, 0);
				}
				return mOperandValueSingle.Value;
			}
		}

		private Double? mOperandValueDouble;
		public Double OperandValueDouble {
			get {
				if (mOperandValueDouble == null) {
					mOperandValueDouble = BitConverter.ToDouble(Operand, 0);
				}
				return mOperandValueDouble.Value;
			}
		}

		public bool Read() {
			Position = NextPosition;
			int xByteValueInt = mStream.ReadByte();
			OpCodeEnum xOpCode;
			if (xByteValueInt == -1) {
				return false;
			}
			byte xByteValue = (byte)xByteValueInt;
			if (xByteValue == 0xFE) {
				xByteValueInt = mStream.ReadByte();
				if (xByteValueInt == -1) {
					return false;
				}
				xOpCode = (OpCodeEnum)(xByteValue << 8 | xByteValueInt);
			} else {
				xOpCode = (OpCodeEnum)xByteValue;
			}
			byte xOperandSize = GetOperandSize(xOpCode);
			mOperand = null;
			mOperandValueStr = null;
			mOperandValueMethod = null;
			mOperandValueField = null;
			mOperandValueSingle = null;
			mOperandValueType = null;
			mOperandValueInt32 = null;
			mOperandValueBranchPosition = null;
			OperandValueBranchLocations = null;
			mOperandValueDouble = null;
			mOpCode = GetNonShortcutOpCode(xOpCode);
			mIsShortcut = mOpCode != xOpCode;
			mHasOperand = xOperandSize > 0;
			if (mHasOperand) {
				mOperand = ReadOperand(xOperandSize);
				mOperandValueInt32 = GetInt32FromOperandByteArray(mOperand);
			} else {
				if (mOpCode != xOpCode) {
					long? xTempOperand = GetShortcutOperand(xOpCode);
					if (xTempOperand != null) {
						mHasOperand = true;
						mOperand = BitConverter.GetBytes(xTempOperand.Value);
					}
				}
				if (mOpCode == OpCodeEnum.Switch) {
					int[] xBranchLocations1 = new int[ReadInt32()];
					for (int i = 0; i < xBranchLocations1.Length; i++) {
						xBranchLocations1[i] = ReadInt32();
					}
					uint[] xResult = new uint[xBranchLocations1.Length];
					for (int i = 0; i < xBranchLocations1.Length; i++) {
						if ((NextPosition + xBranchLocations1[i]) < 0) {
							xResult[i] = (uint)xBranchLocations1[i];
						} else {
							xResult[i] = (uint)(NextPosition + xBranchLocations1[i]);
						}
					}
					OperandValueBranchLocations = xResult;
				}
			}
			return true;
		}


		private Int64 ReadInt64() {
			long xResult = 0;
			byte xOperandSize = 64;
			byte[] xBytes = new byte[xOperandSize / 8];
			while (xOperandSize > 0) {
				int xByteValueInt = mStream.ReadByte();
				if (xByteValueInt == -1) {
					break;
				}
				xBytes[(xOperandSize / 8) - 1] = (byte)xByteValueInt;
				xOperandSize -= 8;
			}
			for (int i = 0; i < xBytes.Length; i++) {
				xResult = xResult << 8 | xBytes[i];
			}
			return xResult;
		}

		private byte[] ReadOperand(byte aOperandSize) {
			byte[] xBytes = new byte[aOperandSize / 8];
			int index = 0;
			while (aOperandSize > 0) {
				int xByteValueInt = mStream.ReadByte();
				if (xByteValueInt == -1) {
					break;
				}
				xBytes[index] = (byte)xByteValueInt;
				index += 1;
				aOperandSize -= 8;
			}
			return xBytes;
		}

		private static Int32 GetInt32FromOperandByteArray(byte[] aData) {
			Int32 xResult = 0;
			for (int i = aData.Length - 1; i >= 0; i--) {
				xResult = xResult << 8 | aData[i];
			}
			return xResult;
		}

		private Int32 ReadInt32() {
			return GetInt32FromOperandByteArray(ReadOperand(32));
		}
	}
}
#endif
