#pragma once

#include <algorithm>
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

		// Get one of the found sources with pattern matching.
		NDIlib_source_t getSourceWithClause(const char* clause) const
		{
			// Retrieve the current list of the sources.
			uint32_t count;
			auto sources = NDIlib_find_get_current_sources(finder_, &count);

			// Return null source if there is no source.
			if (count == 0) return NDIlib_source_t();

			// return the first source if no clause was given.
			if (clause == nullptr || clause[0] == 0) return sources[0];
			
			// Scan the source list.
			for (uint32_t i = 0; i < count; i++)
				if (std::string(sources[i].p_ndi_name).find(clause) != std::string::npos)
					return sources[i];

			// Nothing found: Return null source.
			return NDIlib_source_t();
		}

		// Retrieve the name list of the found sources.
		int retrieveSourceNames(const char* names[], int maxNames) const
		{
			// Retrieve the current list of the sources.
			uint32_t count;
			auto sources = NDIlib_find_get_current_sources(finder_, &count);

			// Copy these names up to maxNames.
			count = std::min(count, static_cast<uint32_t>(maxNames));
			for (uint32_t i = 0; i < count; i++) names[i] = sources[i].p_ndi_name;

			return count;
		}

	private:

		NDIlib_find_instance_t finder_;

		Finder()
		{
			// Create a finder instance and let it run in an independent thread.
			finder_ = NDIlib_find_create_v2(&NDIlib_find_create_t());
			std::thread(&Finder::FinderThread, this).detach();
		}

		void FinderThread()
		{
			while (true) NDIlib_find_wait_for_sources(finder_, 5000);
		}
	};
}
