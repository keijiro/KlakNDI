#pragma once

#include <thread>
#include <Processing.NDI.Lib.h>

namespace KlakNDI
{
	// NDI singleton finder class
	class Finder
	{
	public:

		// Get the singleton instance.
		static Finder& getInstance()
		{
			static Finder instance;
			return instance;
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

		Finder()
		{
			finder_ = NDIlib_find_create_v2(&NDIlib_find_create_t());
			std::thread(&Finder::FinderThread, this).detach();
		}

		void FinderThread()
		{
			while (true) NDIlib_find_wait_for_sources(finder_, 5000);
		}
	};
}
