#pragma once

#include <Processing.NDI.Lib.h>

namespace KlakNDI
{
	// NDI sender class
	class Sender
	{
	public:

		Sender(const char* name)
		{
			instance_ = NDIlib_send_create(&NDIlib_send_create_t(name));
		}

		~Sender()
		{
			NDIlib_send_destroy(instance_);
		}

		void sendFrame(void* data, int width, int height)
		{
			NDIlib_video_frame_v2_t frame;

			frame.xres = width;
			frame.yres = height;
			frame.FourCC = NDIlib_FourCC_type_RGBX;
			frame.frame_format_type = NDIlib_frame_format_type_progressive;
			frame.frame_rate_N = 30000;
			frame.frame_rate_D = 1001;
			frame.picture_aspect_ratio = 16.0f / 9.0f;
			frame.timecode = 0LL;
			frame.p_data = static_cast<uint8_t*>(data);
			frame.line_stride_in_bytes = width * 4;

			NDIlib_send_send_video_v2(instance_, &frame);
		}

	private:

		NDIlib_send_instance_t instance_;
	};
}
