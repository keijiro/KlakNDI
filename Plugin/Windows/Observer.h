#pragma once

#include <thread>
#include <Processing.NDI.Lib.h>

namespace KlakNDI
{
	// NDI global observer class
	class Observer
	{
	public:

		// Get the singleton instance.
		static Observer& getInstance()
		{
			static Observer instance;
			return instance;
		}

		// Constructor
		Observer()
		{
			NDIlib_initialize();
			finder_ = NDIlib_find_create_v2(&NDIlib_find_create_t());
			finderThread_ = std::thread(&Observer::FinderThread, this);
		}

		// Get a found source.
		NDIlib_source_t getFoundSource() const
		{
			uint32_t count;
			auto sources = NDIlib_find_get_current_sources(finder_, &count);
			if (count == 0) return NDIlib_source_t();
			return sources[0];
		}

	private:

		NDIlib_find_instance_t finder_;
		std::thread finderThread_;

		void FinderThread()
		{
			while (true) NDIlib_find_wait_for_sources(finder_, 5000);
		}
	};
}
