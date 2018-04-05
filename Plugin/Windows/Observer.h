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

		// Initialize and start observing.
		void start()
		{
			// If the previous instance is still running, wait for its termination.
			if (finderThread_.joinable())
			{
				finderShouldStop_ = true;
				finderThread_.join();
			}

			// Start a new observer thread.
			finderShouldStop_ = false;
			finderThread_ = std::thread(&Observer::ObserverThread, this);
		}

		// Request stopping the observer.
		void stop()
		{
			finderShouldStop_ = true;
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
		bool finderShouldStop_;

		void ObserverThread()
		{
			NDIlib_initialize();

			finder_ = NDIlib_find_create_v2(&NDIlib_find_create_t());
			if (finder_ == nullptr) return;

			while (!finderShouldStop_)
				NDIlib_find_wait_for_sources(finder_, 100);

			NDIlib_find_destroy(finder_);
			finder_ = nullptr;

			NDIlib_destroy();
		}
	};
}
