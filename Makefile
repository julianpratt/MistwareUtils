retest:
		dotnet build
		dotnet build test
		dotnet run   --project test/MistwareUtilsTest.csproj 	
		rm -f -r test/bin
		rm -f -r test/obj
		rm -f -r lib
		rm -f -r obj		

release: 
		dotnet build
		dotnet pack
		mv lib/*.nupkg .   
		rm -f -r lib
		rm -f -r obj		
