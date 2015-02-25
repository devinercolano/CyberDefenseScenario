#
# Cookbook Name:: blueteam_default.rb
# Recipe:: default
#
# setup the blueteam instance for the
# cyber defense scenario

# download the submission utility
remote_file "SubmissionDaemon" do
  source "https://github.com/clampz/CyberDefenseScenario/raw/master/blue_team_submitter/SubmissionDaemon.exe"
  path "C:\\Users\\Administrator\\Documents\\SubmissionDaemon.exe"
end

# and its installer
remote_file "submission_install_script" do
  source "https://github.com/clampz/CyberDefenseScenario/raw/master/blue_team_submitter/submission_install.bat"
  path "C:\\Users\\Administrator\\Documents\\submission_install.bat"
end

# plus an uninstaller - just in case
remote_file "submission_uninstall_script" do
  source "https://github.com/clampz/CyberDefenseScenario/raw/master/blue_team_submitter/submission_uninstall.bat"
  path "C:\\Users\\Administrator\\Documents\\submission_uninstall.bat"
end

# also install start the utility since it runs in the background
powershell_script "install_start_submission_sv" do
  cwd "C:\\Users\\Administrator\\Documents"
  code <<-EOH
  Unblock-File SubmissionDaemon.exe
  .\\submission_install.bat
  sasv "SubmissionDaemon"
  EOH
end
