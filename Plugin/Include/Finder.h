#pragma once

#include <algorithm>
#include <thread>

namespace KlakNDI
{
    // NDI source finder class (singleton)
    class Finder
    {
    public:

        static Finder& getInstance()
        {
            static Finder instance;
            return instance;
        }

        // Retrieve names of available NDI sources into a given array.
        int retrieveSourceNames(const char* names[], int maxNames) const
        {
            // Retrieve the current source list.
            uint32_t count;
            auto sources = NDIlib_find_get_current_sources(finder_, &count);

            // Copy the source names into the given array.
            count = std::min(count, static_cast<uint32_t>(maxNames));
            for (uint32_t i = 0; i < count; i++) names[i] = sources[i].p_ndi_name;

            return count;
        }

    private:

        NDIlib_find_instance_t finder_;

        Finder()
        {
            // Create a finder instance and let it run on an independent thread.
            NDIlib_find_create_t find;
            finder_ = NDIlib_find_create_v2(&find);
            std::thread(&Finder::FinderThread, this).detach();
        }

        void FinderThread()
        {
            while (true) NDIlib_find_wait_for_sources(finder_, 5000);
        }
    };
}
