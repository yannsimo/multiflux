cmake_minimum_required(VERSION 3.15)

project(PricerEngine C CXX)

# Release or Debug
if (NOT CMAKE_BUILD_TYPE)
    message(STATUS "Setting build type to 'Debug' as none was specified.")
    set(CMAKE_BUILD_TYPE Debug CACHE STRING "Choose the type of build." FORCE)
endif ()

list(APPEND CMAKE_MODULE_PATH "${PROJECT_SOURCE_DIR}/cmake")
# Find Protobuf installation
# Must use the CONFIG mode
# The block command was introduced in 3.25
# block()
#     set(CMAKE_FIND_PACKAGE_PREFER_CONFIG ON)
#     find_package(Protobuf REQUIRED CONFIG)
# endblock()
if (DEFINED CMAKE_FIND_PACKAGE_PREFER_CONFIG)
    set(CMAKE_FIND_PACKAGE_PREFER_CONFIG_VALUE ${CMAKE_FIND_PACKAGE_PREFER_CONFIG})
endif()
set(CMAKE_FIND_PACKAGE_PREFER_CONFIG ON)
find_package(Protobuf REQUIRED CONFIG)
unset(CMAKE_FIND_PACKAGE_PREFER_CONFIG)
if (DEFINED CMAKE_FIND_PACKAGE_PREFER_CONFIG_VALUE)
    set(CMAKE_FIND_PACKAGE_PREFER_CONFIG ${CMAKE_FIND_PACKAGE_PREFER_CONFIG_VALUE})
endif()


# unset(CMAKE_FIND_PACKAGE_PREFER_CONFIG)
message(STATUS "Using protobuf ${Protobuf_VERSION}")

set(_PROTOBUF_LIBPROTOBUF protobuf::libprotobuf)
set(_REFLECTION gRPC::grpc++_reflection)
set(_PROTOBUF_PROTOC $<TARGET_FILE:protobuf::protoc>)

# Find gRPC installation
# Looks for gRPCConfig.cmake file installed by gRPC's cmake installation.
find_package(gRPC CONFIG REQUIRED)
message(STATUS "Using gRPC ${gRPC_VERSION}")

set(_GRPC_GRPCPP gRPC::grpc++)
set(_GRPC_CPP_PLUGIN_EXECUTABLE $<TARGET_FILE:gRPC::grpc_cpp_plugin>)

# Proto file
get_filename_component(pricer_proto "pricing.proto" ABSOLUTE)
get_filename_component(pricer_proto_path "${pricer_proto}" PATH)

# Generated sources
set(pricer_proto_srcs "${CMAKE_CURRENT_BINARY_DIR}/pricing.pb.cc")
set(pricer_proto_hdrs "${CMAKE_CURRENT_BINARY_DIR}/pricing.pb.h")
set(pricer_grpc_srcs "${CMAKE_CURRENT_BINARY_DIR}/pricing.grpc.pb.cc")
set(pricer_grpc_hdrs "${CMAKE_CURRENT_BINARY_DIR}/pricing.grpc.pb.h")
add_custom_command(
    OUTPUT "${pricer_proto_srcs}" "${pricer_proto_hdrs}" "${pricer_grpc_srcs}" "${pricer_grpc_hdrs}"
    COMMAND ${_PROTOBUF_PROTOC}
    ARGS --grpc_out "${CMAKE_CURRENT_BINARY_DIR}"
      --cpp_out "${CMAKE_CURRENT_BINARY_DIR}"
      -I "${pricer_proto_path}"
      --plugin=protoc-gen-grpc="${_GRPC_CPP_PLUGIN_EXECUTABLE}"
      "${pricer_proto}"
    DEPENDS "${pricer_proto}"
)

# Find Pnl
find_package(Pnl REQUIRED)
set(LIBS ${LIBS} ${PNL_LIBRARIES})
include_directories(${PNL_INCLUDE_DIRS})
if(${CMAKE_BUILD_TYPE} STREQUAL "Release")
    add_definitions(-DPNL_RANGE_CHECK_OFF)
endif()

# Find nlohmann_json to parse json files
# Add nlohmann_json::nlohmann_json to target_link_libraries
find_package(nlohmann_json REQUIRED)

# Include generated *.pb.h files
include_directories("${CMAKE_CURRENT_BINARY_DIR}")

# pricer_grpc_proto
add_library(pricer_grpc_proto
    ${pricer_grpc_srcs}
    ${pricer_grpc_hdrs}
    ${pricer_proto_srcs}
    ${pricer_proto_hdrs}
)
target_link_libraries(pricer_grpc_proto
    ${_REFLECTION}
    ${_GRPC_GRPCPP}
    ${_PROTOBUF_LIBPROTOBUF}
)

# add_executable(BlackScholesModelTest BlackScholesModel.cpp BlackScholesModelTest.cpp RandomGeneration.cpp MonteCarloRoutine.cpp ConditionalBasketOption.cpp Option.cpp pricer.cpp)
# target_link_libraries(BlackScholesModelTest
#     ${PNL_LIBRARIES}
#     ${_REFLECTION}
#     ${_GRPC_GRPCPP}
#     ${_PROTOBUF_LIBPROTOBUF}
#     nlohmann_json::nlohmann_json

# )

add_executable(pricing_server pricing_server.cpp pricer.cpp json_reader.cpp BlackScholesModel.cpp  RandomGeneration.cpp MonteCarloRoutine.cpp ConditionalBasketOption.cpp Option.cpp pricer.cpp ConditionalMaxOption.cpp)
target_link_libraries(pricing_server
    pricer_grpc_proto
    ${_REFLECTION}
    ${_GRPC_GRPCPP}
    ${_PROTOBUF_LIBPROTOBUF}
    ${PNL_LIBRARIES}
    nlohmann_json::nlohmann_json
)
pnl_add_postbuild(pricing_server)

add_executable(test_json_reader json_reader.cpp test_json_reader.cpp)
target_link_libraries(test_json_reader
    ${PNL_LIBRARIES}
    nlohmann_json::nlohmann_json
)
pnl_add_postbuild(test_json_reader)


