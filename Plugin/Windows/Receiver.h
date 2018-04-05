#pragma once

#include <Processing.NDI.Lib.h>

namespace KlakNDI
{
	// NDI receiver class
	class Receiver
	{
	public:

		Receiver(const NDIlib_source_t& source)
		{
			instance_ = NDIlib_recv_create_v2(&NDIlib_recv_create_t(source));
		}

		~Receiver()
		{
			NDIlib_recv_destroy(instance_);
		}

		bool receiveFrame()
		{
			auto type = NDIlib_recv_capture_v2(instance_, &frame_, nullptr, nullptr, 0);
			return type == NDIlib_frame_type_video;
		}

		void freeFrame()
		{
			NDIlib_recv_free_video_v2(instance_, &frame_);
		}

		int getFrameWidth() const
		{
			return frame_.xres;
		}

		int getFrameHeight() const
		{
			return frame_.yres;
		}

		void* getFrameData() const
		{
			return frame_.p_data;

		}

	private:

		NDIlib_recv_instance_t instance_;
		NDIlib_video_frame_v2_t frame_;
	};
}
