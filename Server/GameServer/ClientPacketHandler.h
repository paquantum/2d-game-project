#pragma once

using PacketHandlerFunc = std::function<bool(PacketSessionRef&, BYTE*, int32)>;
extern PacketHandlerFunc GPacketHandler[UINT16_MAX];

enum : uint16
{
	PKT_C_REGISTER = 1000, // C -> S
	PKT_S_REGISTER = 1001, // S -> C
	PKT_C_LOGIN = 1004,
	PKT_S_LOGIN = 1005,
	PKT_C_CREATE_CHARACTER = 1006,
	PKT_S_CREATE_CHARACTER = 1007,
	PKT_C_ENTER_GAME = 1010,
	PKT_S_ENTER_GAME = 1011,
	PKT_C_MOVE = 1012,
	PKT_S_MOVE = 1013,
	PKT_C_CHAT = 1014,
	PKT_S_CHAT = 1015,
	PKT_C_LOAD_INVENTORY = 1016,
	PKT_S_LOAD_INVENTORY = 1017,
	PKT_C_OTHER_PLAYER = 1018,
	PKT_S_OTHER_PLAYER = 1019,

	PKT_C_TRADE_REQUEST = 1020,
	PKT_S_TRADE_INVITATION = 1021,
	PKT_C_TRADE_RESPONSE = 1022,
	PKT_S_TRADE_RESPONSE = 1023,
	PKT_C_TRADE_ADD_ITEM = 1024,
	PKT_C_TRADE_ADD_GOLD = 1025,
	PKT_S_TRADE_UPDATE = 1026,
	PKT_C_TRADE_CONFIRM = 1027,
	PKT_S_TRADE_COMPLETE = 1028,
	PKT_C_TRADE_CANCEL = 1029,
	PKT_S_TRADE_CANCEL = 1030
};

// Custom Handlers
bool Handle_INVALID(PacketSessionRef& session, BYTE* buffer, int32 len);
bool Handle_C_REGISTER(PacketSessionRef& session, const UserPKT::C_REGISTER& pkt);
bool Handle_C_LOGIN(PacketSessionRef& session, const UserPKT::C_LOGIN& pkt);
bool Handle_C_CREATE_CHARACTER(PacketSessionRef& session, const UserPKT::C_CREATE_CHARACTER& pkt);
bool Handle_C_ENTER_GAME(PacketSessionRef& session, const UserPKT::C_ENTER_GAME& pkt);
bool Handle_C_OTHER_PLAYER(PacketSessionRef& session, const UserPKT::C_OTHER_PLAYER& pkt);
bool Handle_C_LOAD_INVENTORY(PacketSessionRef& session, const UserPKT::C_LOAD_INVENTORY& pkt);
bool Handle_C_MOVE(PacketSessionRef& session, const UserPKT::C_MOVE& pkt);
bool Handle_C_CHAT(PacketSessionRef& session, const UserPKT::C_CHAT& pkt);
// 거래 패킷
bool Handle_C_TRADE_REQUEST(PacketSessionRef& session, const UserPKT::C_TRADE_REQUEST& pkt);
bool Handle_C_TRADE_RESPONSE(PacketSessionRef& session, const UserPKT::C_TRADE_RESPONSE& pkt);
bool Handle_C_TRADE_ADD_ITEM(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_ITEM& pkt);
bool Handle_C_TRADE_ADD_GOLD(PacketSessionRef& session, const UserPKT::C_TRADE_ADD_GOLD& pkt);
bool Handle_C_TRADE_CONFIRM(PacketSessionRef& session, const UserPKT::C_TRADE_CONFIRM& pkt);
bool Handle_C_TRADE_CANCEL(PacketSessionRef& session, const UserPKT::C_TRADE_CANCEL& pkt);

class ClientPacketHandler
{
public:
	static void Init()
	{
		for (int32 i = 0; i < UINT16_MAX; i++)
			GPacketHandler[i] = Handle_INVALID;

		GPacketHandler[PKT_C_REGISTER] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_REGISTER>(Handle_C_REGISTER, session, buffer, len); };
		GPacketHandler[PKT_C_LOGIN] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_LOGIN>(Handle_C_LOGIN, session, buffer, len);};
		GPacketHandler[PKT_C_CREATE_CHARACTER] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_CREATE_CHARACTER>(Handle_C_CREATE_CHARACTER, session, buffer, len); };
		GPacketHandler[PKT_C_ENTER_GAME] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_ENTER_GAME>(Handle_C_ENTER_GAME, session, buffer, len);};
		GPacketHandler[PKT_C_OTHER_PLAYER] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_OTHER_PLAYER>(Handle_C_OTHER_PLAYER, session, buffer, len);};
		GPacketHandler[PKT_C_LOAD_INVENTORY] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_LOAD_INVENTORY>(Handle_C_LOAD_INVENTORY, session, buffer, len);};
		GPacketHandler[PKT_C_CHAT] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_CHAT>(Handle_C_CHAT, session, buffer, len);};
		GPacketHandler[PKT_C_MOVE] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_MOVE>(Handle_C_MOVE, session, buffer, len); };
		// 거래 패킷
		GPacketHandler[PKT_C_TRADE_REQUEST] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_REQUEST>(Handle_C_TRADE_REQUEST, session, buffer, len); };
		GPacketHandler[PKT_C_TRADE_RESPONSE] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_RESPONSE>(Handle_C_TRADE_RESPONSE, session, buffer, len); };
		GPacketHandler[PKT_C_TRADE_ADD_ITEM] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_ADD_ITEM>(Handle_C_TRADE_ADD_ITEM, session, buffer, len); };
		GPacketHandler[PKT_C_TRADE_ADD_GOLD] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_ADD_GOLD>(Handle_C_TRADE_ADD_GOLD, session, buffer, len); };
		GPacketHandler[PKT_C_TRADE_CONFIRM] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_CONFIRM>(Handle_C_TRADE_CONFIRM, session, buffer, len); };
		GPacketHandler[PKT_C_TRADE_CANCEL] = [](PacketSessionRef& session, BYTE* buffer, int32 len)
			{ return HandlePacket<UserPKT::C_TRADE_CANCEL>(Handle_C_TRADE_CANCEL, session, buffer, len); };

	}

	static bool HandlePacket(PacketSessionRef& session, BYTE* buffer, int32 len)
	{
		PacketHeader* header = reinterpret_cast<PacketHeader*>(buffer);
		return GPacketHandler[header->id](session, buffer, len);
	}

	// 자동화
	static SendBufferRef MakeSendBuffer(uint8* buffer, size_t size, uint16 pktId) { return _MakeSendBuffer(buffer, size, pktId); }
	static SendBufferRef MakeSendBuffer(flatbuffers::FlatBufferBuilder& builder, uint16 pktId) { return _MakeSendBuffer2(builder, pktId); }

private:
	// 받는 역할의 핵심
	template <typename PacketType, typename ProcessFunc>
	static bool HandlePacket(ProcessFunc func, PacketSessionRef& session, BYTE* buffer, int32 len) {
		//if (len < sizeof(PacketType)) { // 예외 체크
		//	std::cerr << "Invalid packet size for type: " << typeid(PacketType).name() << std::endl;
		//	return false;
		//}

		//PacketType* pkt = reinterpret_cast<PacketType*>(buffer);
		auto packet = flatbuffers::GetRoot<PacketType>(buffer + sizeof(PacketHeader));

		// 전달된 핸들러 호출
		return func(session, *packet);
		//return true;
	}

	// 보내는 역할의 핵심
	//template<typename T>
	static SendBufferRef _MakeSendBuffer(uint8* buffer, size_t size, uint16 pktId)
	{
		const uint16 packetSize = size + sizeof(PacketHeader);
		SendBufferRef sendBuffer = GSendBufferManager->Open(packetSize);

		PacketHeader* header = reinterpret_cast<PacketHeader*>(sendBuffer->Buffer());
		header->id = pktId;
		header->size = packetSize;

		std::memcpy(&header[1], buffer, size);
		sendBuffer->Close(header->size);

		return sendBuffer;
	}

	static SendBufferRef _MakeSendBuffer2(flatbuffers::FlatBufferBuilder& builder, uint16 pktId)
	{
		uint8_t* buffer = builder.GetBufferPointer();
		size_t size = builder.GetSize();

		const uint16 packetSize = size + sizeof(PacketHeader);
		SendBufferRef sendBuffer = GSendBufferManager->Open(packetSize);

		PacketHeader* header = reinterpret_cast<PacketHeader*>(sendBuffer->Buffer());
		header->id = pktId;
		header->size = packetSize;

		std::memcpy(&header[1], buffer, size);
		//std::cout << "FlatBuffers serialization completed. Buffer size: " << size << " bytes\n";
		sendBuffer->Close(header->size);

		return sendBuffer;
	}
};