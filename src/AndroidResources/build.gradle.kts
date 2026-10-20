// Top-level build file where you can add configuration options common to all sub-projects/modules.
plugins {
    alias(libs.plugins.android.library) apply false
    alias(libs.plugins.kotlin.android) apply false
}

subprojects {
    afterEvaluate {
        plugins.withId("com.android.library") {
            val namespace = (extensions.findByName("android") as? com.android.build.gradle.LibraryExtension)?.namespace ?: "unknown"

            configurePackagingRule()
            val renameArrFileTaskName = registerRenameAarFileTask(namespace)
            val outputSoFilesTaskName = registerOutputSoFilesTask()

            tasks.named("build").configure {
                if (renameArrFileTaskName != null)
                    dependsOn(renameArrFileTaskName)

                if (outputSoFilesTaskName != null)
                    dependsOn(outputSoFilesTaskName)
            }
        }
    }
}

fun Project.configurePackagingRule() {
    extensions.findByName("android")?.let {
        (it as? com.android.build.gradle.BaseExtension)?.apply {
            packagingOptions {
                jniLibs.excludes += "**/*_Native.so"
            }
        }
    }
}

fun Project.registerRenameAarFileTask(namespace: String): String? {
    val taskName = "renameAarFiles_${name}"
    if (tasks.names.contains(taskName)) {
        return null
    }

    tasks.register(taskName) {
        dependsOn("assembleDebug", "assembleRelease")

        doLast {
            val aarOutputDir = layout.buildDirectory.dir("outputs/aar").get().asFile
            aarOutputDir.listFiles()
                ?.filter { it.name.endsWith(".aar") }
                ?.forEach {
                    val buildType = when {
                        it.name.contains("-debug") -> "debug"
                        it.name.contains("-release") -> "release"
                        else -> "unknown"
                    }

                    if (buildType == "unknown")
                        return@forEach

                    val targetAar = File(aarOutputDir, "${namespace}_${buildType}.aar")
                    if (targetAar.exists()) {
                        targetAar.delete()
                    }

                    it.renameTo(targetAar)
                    println("AAR renamed: ${targetAar.absolutePath}")
                }
        }
    }

    return taskName

}

fun Project.registerOutputSoFilesTask(): String? {
    val taskName = "outputSoFiles_${name}"
    if (tasks.names.contains(taskName)) {
        return null
    }

    tasks.register(taskName).configure {
        dependsOn("assembleDebug", "assembleRelease")

        doLast {
            val outputDir = layout.buildDirectory.dir("outputs/so").get().asFile

            val soFiles = fileTree(layout.buildDirectory.dir("intermediates/cxx").get().asFile)
                .matching { include("**/obj/**/lib*.so") }
                .files

            soFiles.forEach { soFile ->
                val parts = soFile.absolutePath.split(File.separator)

                val buildTypeRaw =
                    parts.firstOrNull {
                        it.equals("RelWithDebInfo", true) || it.equals(
                            "Debug",
                            true
                        )
                    }
                val buildType = when (buildTypeRaw?.lowercase()) {
                    "relwithdebinfo" -> "Release"
                    "debug" -> "Debug"
                    else -> "Unknown"
                }

                val abi = parts.getOrNull(parts.indexOf("obj") + 1) ?: "unknown"

                val destinationDir = File(outputDir, "$buildType/$abi")
                if (!destinationDir.exists())
                    destinationDir.mkdirs()

                val destinationFile = File(destinationDir, soFile.name)
                soFile.copyTo(destinationFile, overwrite = true)

                println("SO copied: $soFile -> $destinationFile")
            }
        }
    }

    return taskName
}
